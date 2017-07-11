using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using Akka.Actor;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.CommonDomain;
using GridDomain.Node.Actors.Aggregates.Exceptions;
using GridDomain.Node.Actors.Aggregates.Messages;
using GridDomain.Node.Actors.CommandPipe.Messages;
using GridDomain.Node.Actors.EventSourced;
using GridDomain.Node.Actors.EventSourced.Messages;
using GridDomain.Node.Actors.Sagas.Messages;

namespace GridDomain.Node.Actors.Aggregates
{
    /// <summary>
    ///     Name should be parse by AggregateActorName
    /// </summary>
    /// <typeparam name="TAggregate"></typeparam>
    public class AggregateActor<TAggregate> : EventSourcedActor<TAggregate> where TAggregate : EventSourcing.Aggregate
    {
        private readonly IActorRef _customHandlersActor;
        private readonly ProcessEntry _domainEventProcessEntry;
        private readonly ProcessEntry _domainEventProcessFailEntry;
        private readonly IPublisher _publisher;

        private readonly IAggregateCommandsHandler<TAggregate> _aggregateCommandsHandler;
        private readonly List<IActorRef> _commandCompletedWaiters = new List<IActorRef>();
        private AggregateCommandExecutionContext<TAggregate> ExecutionContext { get; } = new AggregateCommandExecutionContext<TAggregate>();

        public AggregateActor(IAggregateCommandsHandler<TAggregate> handler,
                              IActorRef schedulerActorRef,
                              IPublisher publisher,
                              ISnapshotsPersistencePolicy snapshotsPersistencePolicy,
                              IConstructAggregates aggregateConstructor,
                              IActorRef customHandlersActor) : base(aggregateConstructor, snapshotsPersistencePolicy)
        {
            _aggregateCommandsHandler = handler;
            _publisher = publisher;
            _customHandlersActor = customHandlersActor;
            _domainEventProcessEntry = new ProcessEntry(Self.Path.Name, AggregateActorConstants.PublishingEvent, AggregateActorConstants.CommandExecutionCreatedAnEvent);
            _domainEventProcessFailEntry = new ProcessEntry(Self.Path.Name, AggregateActorConstants.CreatedFault, AggregateActorConstants.CommandRaisedAnError);
            State.SetPersistProvider(PersistEventPack(Self));
            Behavior.Become(AwaitingCommandBehavior, nameof(AwaitingCommandBehavior));
        }

        protected virtual void AwaitingCommandBehavior()
        {
            DefaultBehavior();
            Command<NotifyOnCommandComplete>(n =>
                                             {
                                                 _commandCompletedWaiters.Add(Sender);
                                                 Sender.Tell(NotifyOnCommandCompletedAck.Instance);
                                             });

            Command<IMessageMetadataEnvelop<ICommand>>(m =>
                                                       {
                                                           var cmd = m.Message;
                                                           Monitor.Increment(nameof(CQRS.Command));
                                                           ExecutionContext.Command = cmd;
                                                           ExecutionContext.CommandMetadata = m.Metadata;

                                                           _aggregateCommandsHandler.ExecuteAsync(State,
                                                                                                  cmd,
                                                                                                  PersistEventPack(Self))
                                                                                    .ContinueWith(t =>
                                                                                                  {
                                                                                                      if (t.IsFaulted) throw t.Exception;
                                                                                                      return CommandProducedEventsPersisted.Instance;
                                                                                                  })
                                                                                    .PipeTo(Self);

                                                           Behavior.Become(ProcessingCommandBehavior, nameof(ProcessingCommandBehavior));
                                                       });
        }

        private PersistenceDelegate PersistEventPack(IActorRef self)
        {
            return agr =>
                   {
                       ExecutionContext.ProducedState = (TAggregate) agr;
                       return self.Ask<EventsPersisted>(new PersistEventPack(agr.GetDomainEvents()));
                   };
        }

        private void ProcessingCommandBehavior()
        {
            var producedEventsMetadata = ExecutionContext.CommandMetadata.CreateChild(Id, _domainEventProcessEntry);

            //just for catching Failures on events persist
            Command<PersistEventPack>(e =>
                                   {
                                       Monitor.Increment(nameof(Messages.PersistEventPack));
                                       var domainEvents = e.Events;
                                       if (!domainEvents.Any())
                                       {
                                           Log.Warning("Aggregate {id} trying to persist events but no events is presented", PersistenceId);
                                           return;
                                       }

                                       //dirty hack, but we know nobody will modify domain events before us 
                                       foreach (var evt in domainEvents)
                                           evt.SagaId = ExecutionContext.Command.SagaId;

                                       ExecutionContext.MessagesToProject.AddRange(domainEvents);

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
                                                      TrySaveSnapshot(ExecutionContext.ProducedState);

                                                      if (ExecutionContext.ProducedState.HasUncommitedEvents)
                                                          return;

                                                      Sender.Tell(EventsPersisted.Instance);
                                                  });
                                   });
            //aggregate raised an error during command execution
            Command<Status.Failure>(f => PublishError(f.Cause.UnwrapSingle())
                                        .ContinueWith(t => CommandExecuted.Instance)
                                        .PipeTo(Self));

            Command<CommandProducedEventsPersisted>(newState =>
                                             {
                                                 Log.Debug("{Aggregate} received a {@command}", PersistenceId, newState);

                                                 ExecutionContext.MessagesToProject.Select(e => Project(e, producedEventsMetadata)).
                                                                  ToChain().
                                                                  ContinueWith(t =>
                                                                               {
                                                                                   //Publish produced messages
                                                                                   foreach (var e in ExecutionContext.MessagesToProject)
                                                                                       _publisher.Publish(e, producedEventsMetadata);
                                                                                   return CommandExecuted.Instance;
                                                                               })
                                                                  .PipeTo(Self);
                                             });

            Command<CommandExecuted>(c =>
                                     {
                                         Log.Debug("{Aggregate} received a {@command}", PersistenceId, c);
                                         //finish command execution. produced state can be null on execution error
                                         State = ExecutionContext.ProducedState ?? State;
                                         //notify waiters
                                         foreach(var waiter in _commandCompletedWaiters)
                                             waiter.Tell(new CommandCompleted(ExecutionContext.Command.Id));

                                         ExecutionContext.Clear();
                                         Behavior.Unbecome();
                                         Stash.UnstashAll();
                                     });

            Command<IMessageMetadataEnvelop<ICommand>>(o => StashMessage(o));

            Command<GracefullShutdownRequest>(o => StashMessage(o));

            DefaultBehavior();
        }

        private Task<AllHandlersCompleted> PublishError(Exception exception)
        {
            var command = ExecutionContext.Command;
            var commandMetadata = ExecutionContext.CommandMetadata;
            var commandExecutionException = new CommandExecutionFailedException(command, exception);

            Log.Error(commandExecutionException, "{Aggregate} raised an error while executing {@Command}", PersistenceId, command);

            var producedFaultMetadata = commandMetadata.CreateChild(command.Id, _domainEventProcessFailEntry);
            var fault = Fault.NewGeneric(command, commandExecutionException, command.SagaId, typeof(TAggregate));
            return Project(fault, producedFaultMetadata)
                        .ContinueWith(t =>
                                     {
                                         _publisher.Publish(fault, producedFaultMetadata);
                                         return t.Result;
                                     });
        }

        protected override void TerminatingBehavior()
        {
            Command<IMessageMetadataEnvelop<ICommand>>(c =>
                                                       {
                                                           Self.Tell(CancelShutdownRequest.Instance);
                                                           StashMessage(c);
                                                       });
            base.TerminatingBehavior();
        }

        private Task<AllHandlersCompleted> Project(object evt, IMessageMetadata commandMetadata)
        {
            var envelop = new MessageMetadataEnvelop<Project>(new Project(evt), commandMetadata);
            return _customHandlersActor.Ask<AllHandlersCompleted>(envelop);
        }
    }
}