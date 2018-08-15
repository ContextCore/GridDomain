using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.DI.Core;
using Automatonymous;
using Google.Protobuf.WellKnownTypes;
using GridDomain.Common;
using GridDomain.Configuration;
using GridDomain.Configuration.MessageRouting;
using GridDomain.Configuration.SnapshotPolicies;
using GridDomain.CQRS;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.CommonDomain;
using GridDomain.Node.Actors.Aggregates.Messages;
using GridDomain.Node.Actors.CommandPipe;
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
    public class AggregateActor<TAggregate> : DomainEventSourcedActor<TAggregate> where TAggregate : class, IAggregate
    {
        private readonly IActorRef _customHandlersActor;
        private readonly ProcessEntry _domainEventProcessEntry;
        private readonly ProcessEntry _domainEventProcessFailEntry;
        private readonly ProcessEntry _commandCompletedProcessEntry;
        private readonly IPublisher _publisher;

        private readonly IAggregateCommandsHandler<TAggregate> _aggregateCommandsHandler;
        private ActorMonitor.ITimer _commandTotalExecutionTimer;
        private ActorMonitor.ITimer _totalProjectionTimer;
        private ActorMonitor.ITimer _aggregateExecutionTimer;
        private AggregateCommandExecutionContext ExecutionContext { get; } = new AggregateCommandExecutionContext();

        public AggregateActor(IAggregateCommandsHandler<TAggregate> handler,
                              ISnapshotsPersistencePolicy snapshotsPersistencePolicy,
                              IAggregateFactory aggregateConstructor,
                              IConstructSnapshots snapshotsConstructor,
                              IActorRef customHandlersActor) : base(aggregateConstructor, snapshotsConstructor, snapshotsPersistencePolicy)
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

            Command<IMessageMetadataEnvelop>(m =>
                                             {
                                                 Monitor.Increment("CommandsTotal");
                                                 var cmd = (ICommand) m.Message;
                                                 Log.Debug("Received command {cmdId}",cmd.Id);

                                                 ExecutionContext.Command = cmd;
                                                 ExecutionContext.CommandMetadata = m.Metadata;
                                                 ExecutionContext.CommandSender = Sender;

                                                 _publisher.Publish(m);

                                                 _aggregateExecutionTimer = Monitor.StartMeasureTime("AggregateLogic");
                                                 _commandTotalExecutionTimer = Monitor.StartMeasureTime("CommandTotalExecution");

                                                 _aggregateCommandsHandler.ExecuteAsync(State, ExecutionContext.Command)
                                                                          .ContinueWith(t =>
                                                                                        {
                                                                                            _aggregateExecutionTimer.Stop();
                                                                                            ExecutionContext.ProducedState = t.Result;
                                                                                            var domainEvents = ExecutionContext.ProducedState.GetUncommittedEvents();
                                                                                            return domainEvents;
                                                                                        })
                                                                          .PipeTo(Self);

                                                 Behavior.Become(ProcessingCommandBehavior, nameof(ProcessingCommandBehavior));
                                             },
                                             m => m.Message is ICommand);
        }

        protected override bool CanShutdown(out string description)
        {
            if (!ExecutionContext.InProgress) return base.CanShutdown(out description);

            description = $"Command {ExecutionContext.Command.Id} is in progress";
            return false;
        }

        private void ProcessingCommandBehavior()
        {
            var producedEventsMetadata = ExecutionContext.CommandMetadata.CreateChild(Id, _domainEventProcessEntry);
            Command<IReadOnlyCollection<DomainEvent>>(domainEvents =>
                                {
                                    Log.Debug("command executed, starting to persist events");
                                  
                                    if (!domainEvents.Any())
                                    {
                                        Log.Warning("Trying to persist events but no events is presented. {@context}", ExecutionContext);
                                        return;
                                    }

                                    foreach (var evt in domainEvents)
                                        evt.ProcessId = ExecutionContext.Command.ProcessId;
                                    int messagesToPersistCount = domainEvents.Count;
                                    _totalProjectionTimer = null;

                                    var totalPersistenceTimer = Monitor.StartMeasureTime("AggregatePersistence");
                                    PersistAll(domainEvents,
                                               persistedEvent =>
                                               {
                                                   NotifyPersistenceWatchers(persistedEvent);
                                                   _totalProjectionTimer = _totalProjectionTimer ?? Monitor.StartMeasureTime("ProjectionTotal");
                                                   Project(producedEventsMetadata, new[] {persistedEvent});
                                                   SaveSnapshot(ExecutionContext.ProducedState, persistedEvent);

                                                   if (--messagesToPersistCount != 0) return;
                                                   totalPersistenceTimer.Stop();
                                                   CompleteExecution();
                                               });
                                });

            Command<AllHandlersCompleted>(c =>
                                          {
                                              ExecutionContext.MessagesToProject--;
                                              if (ExecutionContext.Projecting) return;

                                              _totalProjectionTimer.Stop();
                                              ExecutionContext.CommandSender.Tell(AggregateActor.CommandProjected.Instance);
                                              WaitForNewCommand();
                                          });
            //aggregate raised an error during command execution
            Command<Status.Failure>(f =>
                                    {
                                        _aggregateExecutionTimer.Stop();
                                        ExecutionContext.Exception = f.Cause.UnwrapSingle();
                                        PublishError(ExecutionContext.Exception);

                                        Behavior.Become(() =>
                                                        {
                                                            Command<AllHandlersCompleted>(c =>
                                                                                              throw new CommandExecutionFailedException(ExecutionContext.Command, ExecutionContext.Exception));
                                                            CommandAny(StashMessage);
                                                        },
                                                        "Waiting for command exception projection");
                                    });

            DefaultBehavior();

            CommandAny(o =>
                       {
                           StashMessage(o);
                       });
        }

        private void CompleteExecution()
        {
            Log.Info("Command executed. {@context}", ExecutionContext.CommandMetadata);

            State = ExecutionContext.ProducedState as TAggregate;
            if (State == null)
                throw new InvalidOperationException("Aggregate state was null after command execution");

            State.ClearUncommitedEvents();

            var completedMetadata = ExecutionContext.CommandMetadata
                                                    .CreateChild(ExecutionContext.Command.Id, _commandCompletedProcessEntry);

            _publisher.Publish(AggregateActor.CommandExecuted.Instance, completedMetadata);

            ExecutionContext.CommandSender.Tell(AggregateActor.CommandExecuted.Instance);

            //waiting to some events been projecting
            if (ExecutionContext.Projecting)
                return;

            WaitForNewCommand();
        }

        private void WaitForNewCommand()
        {
            _commandTotalExecutionTimer?.Stop();
            ExecutionContext.Clear();
            Behavior.Become(AwaitingCommandBehavior, nameof(AwaitingCommandBehavior));
            Stash.Unstash();
        }

        private IFault PublishError(Exception exception)
        {
            var command = ExecutionContext.Command;

            Log.Error(exception, "An error occured while command execution. {@context}", ExecutionContext);

            var producedFaultMetadata = ExecutionContext.CommandMetadata.CreateChild(command.Id, _domainEventProcessFailEntry);
            var faultId = "command_fault_" + Guid.NewGuid();

            var fault = Fault.NewGeneric(faultId,command, exception, command.ProcessId, typeof(TAggregate));

            Project(producedFaultMetadata, fault);
            ExecutionContext.CommandSender.Tell(fault);
            return fault;
        }

        protected virtual void Project(IMessageMetadata metadata, DomainEvent[] events)
        {
            foreach (var evt in events)
                Project(new MessageMetadataEnvelop(evt, metadata));
        }

        protected virtual void Project(IMessageMetadata metadata, IFault fault)
        {
            Project(new MessageMetadataEnvelop(fault, metadata));
        }

        protected void Project(IMessageMetadataEnvelop envelopMessageToProject)
        {
            ExecutionContext.MessagesToProject++;

            _customHandlersActor.Tell(new HandlersPipeActor.Project(Self, envelopMessageToProject));
            _publisher.Publish(envelopMessageToProject);
        }
    }

    public static class AggregateActor
    {
        //Stages of command processing notifications: 
        // 1. Executed
        // 2. Projected

        public class CommandExecuted
        {
            private CommandExecuted() { }
            public static CommandExecuted Instance { get; } = new CommandExecuted();
        }

        public class CommandProjected
        {
            private CommandProjected() { }
            public static CommandProjected Instance { get; } = new CommandProjected();
        }
    }
}