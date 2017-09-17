using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using Akka.Actor;
using GridDomain.Common;
using GridDomain.Configuration;
using GridDomain.Configuration.MessageRouting;
using GridDomain.CQRS;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.CommonDomain;
using GridDomain.Node.Actors.Aggregates.Messages;
using GridDomain.Node.Actors.CommandPipe.Messages;
using GridDomain.Node.Actors.EventSourced;
using GridDomain.Node.Actors.EventSourced.Messages;
using GridDomain.Node.Actors.ProcessManagers.Messages;
using GridDomain.Transport.Extension;

namespace GridDomain.Node.Actors.Aggregates
{
    /// <summary>
    ///     Name should be parse by AggregateActorName
    /// </summary>
    /// <typeparam name="TAggregate"></typeparam>
    public class AggregateActor<TAggregate> : DomainEventSourcedActor<TAggregate> where TAggregate : EventSourcing.Aggregate
    {
        private readonly IActorRef _customHandlersActor;
        private readonly ProcessEntry _domainEventProcessEntry;
        private readonly ProcessEntry _domainEventProcessFailEntry;
        private readonly ProcessEntry _commandCompletedProcessEntry;
        private readonly IPublisher _publisher;

        private readonly IAggregateCommandsHandler<TAggregate> _aggregateCommandsHandler;
        private AggregateCommandExecutionContext ExecutionContext { get; } = new AggregateCommandExecutionContext();

        public AggregateActor(IAggregateCommandsHandler<TAggregate> handler,
                              ISnapshotsPersistencePolicy snapshotsPersistencePolicy,
                              IConstructAggregates aggregateConstructor,
                              IActorRef customHandlersActor) : base(aggregateConstructor, snapshotsPersistencePolicy)
        {
            _aggregateCommandsHandler = handler;
            _publisher = Context.System.GetTransport();
            _customHandlersActor = customHandlersActor;
            _domainEventProcessEntry = new ProcessEntry(Self.Path.Name, AggregateActorConstants.PublishingEvent, AggregateActorConstants.CommandExecutionCreatedAnEvent);
            _domainEventProcessFailEntry = new ProcessEntry(Self.Path.Name, AggregateActorConstants.CommandExecutionFinished, AggregateActorConstants.CommandRaisedAnError);
            _commandCompletedProcessEntry = new ProcessEntry(Self.Path.Name, AggregateActorConstants.CommandExecutionFinished, AggregateActorConstants.ExecutedCommand);
            Behavior.Become(AwaitingCommandBehavior, nameof(AwaitingCommandBehavior));
        }

        protected virtual void AwaitingCommandBehavior()
        {
            DefaultBehavior();
            
            Command<IMessageMetadataEnvelop<ICommand>>(m =>
                                                       {
                                                           var cmd = m.Message;
                                                           Monitor.Increment(nameof(CQRS.Command));

                                                           ExecutionContext.Command = cmd;
                                                           ExecutionContext.CommandMetadata = m.Metadata;
                                                           ExecutionContext.CommandSender = Sender;
                                                           var self = Self;
                                                           Behavior.Become(ProcessingCommandBehavior, nameof(ProcessingCommandBehavior));

                                                           Log.Debug("Executing command. {@m}", ExecutionContext);

                                                           _aggregateCommandsHandler.ExecuteAsync(State,
                                                                                                  cmd,
                                                                                                  agr =>
                                                                                                  {
                                                                                                      ExecutionContext.ProducedState = (TAggregate) agr;
                                                                                                      
                                                                                                      return self.Ask<EventsPersisted>(agr.GetDomainEvents());
                                                                                                  })
                                                                                    .ContinueWith(t =>
                                                                                                  {
                                                                                                      if (t.IsFaulted) throw t.Exception;
                                                                                                      //return CommandProducedEventsPersisted.Instance;
                                                                                                      return CommandExecuted.Instance;
                                                                                                  })
                                                                                    .PipeTo(Self);
                                                       });
        }

        private void ProcessingCommandBehavior()
        {
            var producedEventsMetadata = ExecutionContext.CommandMetadata.CreateChild(Id, _domainEventProcessEntry);
            
            //just for catching Failures on events persist
            Command<IReadOnlyCollection<DomainEvent>>(domainEvents =>
                                   {

                                       Monitor.Increment(nameof(Messages.PersistEventPack));
                                       ExecutionContext.PersistenceWaiter = Sender;
                                       if (!domainEvents.Any())
                                       {
                                           Log.Warning("Trying to persist events but no events is presented. {@context}", ExecutionContext);
                                           return;
                                       }

                                       //dirty hack, but we know nobody will modify domain events before us 
                                       foreach (var evt in domainEvents)
                                           evt.ProcessId = ExecutionContext.Command.ProcessId;

                                       PersistAll(domainEvents,
                                                  persistedEvent =>
                                                  {
                                                      try
                                                      {
                                                          ExecutionContext.ProducedState.MarkPersisted(persistedEvent);
                                                      }
                                                      catch (Exception ex)
                                                      {
                                                          Log.Error(AggregateActorConstants.ErrorOnEventApplyText, Id, ExecutionContext.Command);
                                                          PublishError(ex);
                                                          //intentionally drop all pending commands and messages
                                                          //and don't wait end of projection builders processing as
                                                          //state is corrupted
                                                          Context.Stop(Self);
                                                          return;
                                                      }

                                                      NotifyPersistenceWatchers(persistedEvent);
                                                      SaveSnapshot(ExecutionContext.ProducedState, persistedEvent);
                                                      Project(persistedEvent, producedEventsMetadata);
                                                  });
                                   });

            Command<AllHandlersCompleted>(c =>
                                          {
                                              if (ExecutionContext.Exception == null)
                                              {
                                                  if (ExecutionContext.ProducedState.HasUncommitedEvents)
                                                      return;
                                                  ExecutionContext.PersistenceWaiter.Tell(EventsPersisted.Instance);
                                              }
                                              else
                                              {
                                                  Self.Tell(CommandExecuted.Instance);
                                              }
                                          });
            //aggregate raised an error during command execution
            Command<Status.Failure>(f =>
                                    {
                                        ExecutionContext.Exception = f.Cause.UnwrapSingle();
                                        PublishError(ExecutionContext.Exception);
                                    });

            Command<CommandExecuted>(c =>
                                     {
                                         Log.Debug("Command executed. {@context}", ExecutionContext.CommandMetadata);
                                         //finish command execution. produced state can be null on execution error
                                         State = (TAggregate)ExecutionContext.ProducedState ?? State;
                                         var commandCompleted = new CommandCompleted(ExecutionContext.Command.Id);
                                         var completedMetadata = ExecutionContext.CommandMetadata
                                                                                 .CreateChild(ExecutionContext.Command.Id, _commandCompletedProcessEntry);
                                         _publisher.Publish(commandCompleted, completedMetadata);

                                         ExecutionContext.CommandSender.Tell(commandCompleted);
                                         ExecutionContext.Clear();
                                         Behavior.Unbecome();
                                         Stash.UnstashAll();
                                     });

            Command<IMessageMetadataEnvelop<ICommand>>(o => StashMessage(o));

            Command<GracefullShutdownRequest>(o => StashMessage(o));

            DefaultBehavior();
        }

        private void PublishError(Exception exception)
        {
            var command = ExecutionContext.Command;

            Log.Error(exception, "An error occured while command execution. {@context}", ExecutionContext);

            var producedFaultMetadata = ExecutionContext.CommandMetadata.CreateChild(command.Id, _domainEventProcessFailEntry);
            var fault = Fault.NewGeneric(command, exception, command.ProcessId, typeof(TAggregate));
            Project(fault, producedFaultMetadata);
        }

        private void Project(object evt, IMessageMetadata commandMetadata)
        {
            _customHandlersActor.Tell(new MessageMetadataEnvelop<Project>(new Project(evt), commandMetadata));
        }
    }
}