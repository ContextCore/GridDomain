using System;
using System.Collections.Generic;
using System.Linq;
using Akka;
using Akka.Actor;
using CommonDomain.Persistence;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging;
using GridDomain.CQRS.Messaging.MessageRouting;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.FutureEvents;
using GridDomain.Logging;
using GridDomain.Node.Actors.CommandPipe;
using GridDomain.Scheduling.Akka.Messages;

namespace GridDomain.Node.Actors
{
    class SaveEventsAsync
    {
        public SaveEventsAsync(DomainEvent[] events, Action<DomainEvent> act, Action continuation, Aggregate state)
        {
            Continuation = continuation;
            State = state;
            Events = events;
            Act = act;
        }

        public Action Continuation { get; }
        public DomainEvent[] Events { get; }
        public Aggregate State { get; }
        public Action<DomainEvent> Act { get; }
    }

    //TODO: extract non-actor handler to reuse in tests for aggregate reaction for command
    /// <summary>
    ///     Name should be parse by AggregateActorName
    /// </summary>
    /// <typeparam name="TAggregate"></typeparam>
    public class AggregateActor<TAggregate> : EventSourcedActor<TAggregate> where TAggregate : Aggregate
    {
        public const string CreatedFault = "created fault";
        public const string CommandRaisedAnError = "command raised an error";
        public const string PublishingEvent = "Publishing event";
        public const string CommandExecutionCreatedAnEvent = "Command execution created an event";
        private readonly IActorRef _customHandlersActor;
        private readonly ProcessEntry _domainEventProcessEntry;
        private readonly ProcessEntry _domainEventProcessFailEntry;
        private readonly IPublisher _publisher;
        private readonly IActorRef _schedulerActorRef;

        private readonly IDictionary<Guid, object> _messagesToProject = new Dictionary<Guid, object>();
        private readonly IAggregateCommandsHandler<TAggregate> _aggregateCommandsHandler;

        public new TAggregate State
        {
            get { return (TAggregate) base.State; }
            private set { base.State = value; }
        }

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
            _schedulerActorRef = schedulerActorRef;
            _domainEventProcessEntry = new ProcessEntry(Self.Path.Name, PublishingEvent, CommandExecutionCreatedAnEvent);
            _domainEventProcessFailEntry = new ProcessEntry(Self.Path.Name, CreatedFault, CommandRaisedAnError);

            State.RegisterPersistence((evtTask, onEventPersist, continuation, newState) =>
                                          evtTask.ContinueWith(t => new SaveEventsAsync(t.Result, onEventPersist, continuation, newState))
                                                 .PipeTo(Self));

           AwaitingCommandBehavior();
        }

        protected void AwaitingCommandBehavior()
        {
            Command<IMessageMetadataEnvelop<ICommand>>(m =>
                                                       {
                                                           var cmd = m.Message;
                                                           Monitor.IncrementMessagesReceived();
                                                           Log.Debug("{Aggregate} received a {@command}", PersistenceId, cmd);
                                                           State.RegisterEnricher(e => e.CloneWithSaga(cmd.SagaId));
                                                           _aggregateCommandsHandler.ExecuteAsync(State, cmd)
                                                                  .PipeTo(Self);

                                                           BecomeStacked(() => ProcessingCommandBehavior(m, Sender));
                                                       });
        }

        protected void ProcessingCommandBehavior(IMessageMetadataEnvelop<ICommand> commandEnvelop, IActorRef commandSender)
        {
            var command = commandEnvelop.Message;
            var commandMetadata = commandEnvelop.Metadata;
            var producedEventsMetadata = commandMetadata.CreateChild(Id, _domainEventProcessEntry);

            CommandAny(c => c.Match()
                             //finished some call on aggregate, need persist produced events
                             .With<SaveEventsAsync>(e =>
                                                    {
                                                        bool hasErrorsOnApply = false;
                                                        PersistAll(e.Events,
                                                                   o =>
                                                                   {
                                                                       e.State.MarkPersisted(o);
                                                                       if (hasErrorsOnApply)
                                                                       {
                                                                           return;
                                                                       }

                                                                       TrySaveSnapshot();
                                                                       NotifyPersistenceWatchers(o);
                                                                       Project(o, producedEventsMetadata);

                                                                       try
                                                                       {
                                                                           e.Act(o);
                                                                       }
                                                                       catch (Exception ex)
                                                                       {
                                                                           hasErrorsOnApply = true;
                                                                           Log.Error("Aggregate {id} raised erorrs on events apply after persist while executing command {@command} " +
                                                                                     "State is supposed to be corrupted until code changes." +
                                                                                     "Futher aggregate running is unsafe." +
                                                                                     "Events within same command will be persisted but will not be applied to aggregate to reduce " +
                                                                                     "possible state corruption.", Id, command);

                                                                           Self.Tell(new Status.Failure(ex));
                                                                           //TODO: shutdown without snapshot save
                                                                           return;
                                                                       }

                                                                       if (e.State.IsPendingPersistence)
                                                                           return; 

                                                                       try
                                                                       {
                                                                           e.Continuation();
                                                                           //command execution is finished
                                                                           Self.Tell(e.State);
                                                                       }
                                                                       catch (Exception ex)
                                                                       {
                                                                           hasErrorsOnApply = true;
                                                                           Log.Error("Aggregate {id} raised erorrs on continuation after events apply while executing command {@command} " +
                                                                                    "State is supposed to be corrupted until code changes." +
                                                                                    "Futher aggregate running is unsafe." +
                                                                                    "Events within same command will be persisted but will not be applied to aggregate to reduce " +
                                                                                    "possible state corruption.", Id, command);

                                                                           //TODO: shutdown without snapshot save
                                                                           Self.Tell(new Status.Failure(ex));
                                                                       }
                                                                   });
                                                    })
                             //aggregate raised an error during command execution
                             .With<Failure>(f => HandleError(command, commandMetadata, f.Exception, commandSender))
                             .With<Status.Failure>(f => HandleError(command, commandMetadata, f.Cause, commandSender))
                             //aggregate command execution is finished 
                             //produced events are persisted
                             //we can have events not projected yet
                             .With<TAggregate>(newState =>
                                               {
                                                   //handler finished execution but need to wait for events persistence
                                                   //or aggregate method started execution but don't produce anything yet
                                                   if (newState.IsPendingPersistence || newState.IsMethodExecuting)
                                                       return;

                                                   //can renew state now, as we are waiting only for event projections
                                                   State = newState;
                                                   //projection finished progress, 
                                                   if (!_messagesToProject.Any())
                                                       FinishCommandExecution(command, commandMetadata,commandSender);
                                                   //projection in progress, will finish execution later by notification from message processor
                                               })
                             //projection of event pack from aggregate finished
                             //we can have more event packs to project
                             .With<AllHandlersCompleted>(m =>
                                                         {
                                                             //publish messages for notification
                                                             object projected;
                                                             if (!_messagesToProject.TryGetValue(m.ProjectId, out projected))
                                                                 throw new UnknownProjectionFinishedException();

                                                             _publisher.Publish(projected, producedEventsMetadata);

                                                             //all projections are finished, aggregate events are persisted
                                                             //it means command execution is finished
                                                             if (!_messagesToProject.Any() && !State.IsPendingPersistence)
                                                                 FinishCommandExecution(command, commandMetadata,commandSender);
                                                         })
                             .With<IMessageMetadataEnvelop<ICommand>>(o => Stash.Stash())
                             .With<GracefullShutdownRequest>(o => Stash.Stash())
                             .Default(e => { throw new UnknownMessageReceivedException(); })
                      );
        }

        private void HandleError(ICommand command, IMessageMetadata commandMetadata, Exception exception, IActorRef commandSender)
        {
            var producedFaultMetadata = commandMetadata.CreateChild(command.Id, _domainEventProcessFailEntry);

            var fault = Fault.NewGeneric(command, exception, command.SagaId, typeof(TAggregate));

            Project(fault, producedFaultMetadata);
            Log.Error(exception, "{Aggregate} raised an error {@Exception} while executing {@Command}", PersistenceId, exception, command);
            _publisher.Publish(fault, producedFaultMetadata);
            FinishCommandExecution(command, commandMetadata, commandSender);
        }

        protected virtual void FinishCommandExecution(ICommand cmd, IMessageMetadata metadata, IActorRef commandSender)
        {
            UnbecomeStacked();
            Stash.UnstashAll();
            base.State.ClearUncommittedEvents();
        }

        protected override void TerminatingBehavior()
        {
            Command<IMessageMetadataEnvelop<ICommand>>(c =>
                                                       {
                                                           Self.Tell(CancelShutdownRequest.Instance);
                                                           Stash.Stash();
                                                       });
            base.TerminatingBehavior();
        }

        private void Project(object evt, IMessageMetadata commandMetadata)
        {
            var envelop = new MessageMetadataEnvelop<Project>(new Project(evt), commandMetadata);

            _messagesToProject.Add(envelop.Message.ProjectId, evt);

            evt.Match()
               .With<FutureEventScheduledEvent>(m => Handle(m, commandMetadata))
               .With<FutureEventCanceledEvent>(m => Handle(m, commandMetadata));

            _customHandlersActor.Ask<AllHandlersCompleted>(envelop).PipeTo(Self);
        }

        public void Handle(FutureEventScheduledEvent futureEventScheduledEvent, IMessageMetadata messageMetadata)
        {
            var message = futureEventScheduledEvent;
            var scheduleId = message.Id;
            var aggregateId = message.Event.SourceId;

            var description = $"Aggregate {typeof(TAggregate).Name} id = {aggregateId} scheduled future event "
                              + $"{scheduleId} with payload type {message.Event.GetType().Name} on time {message.RaiseTime}\r\n"
                              + $"Future event: {message.ToPropsString()}";

            var scheduleKey = CreateScheduleKey(scheduleId, aggregateId, description);

            var command = new RaiseScheduledDomainEventCommand(message.Id, message.SourceId, Guid.NewGuid());
            var metadata = messageMetadata.CreateChild(command.Id,
                                                       new ProcessEntry(GetType().Name,
                                                                        "Scheduling raise future event command",
                                                                        "FutureEventScheduled event occured"));

            var confirmationEventType = typeof(IMessageMetadataEnvelop<>).MakeGenericType(message.Event.GetType());

            var scheduleEvent = new ScheduleCommand(command,
                                                    scheduleKey,
                                                    new ExtendedExecutionOptions(message.RaiseTime,
                                                                                 confirmationEventType,
                                                                                 message.Event.SourceId,
                                                                                 nameof(DomainEvent.SourceId)),
                                                    metadata);

            _schedulerActorRef.Tell(scheduleEvent);
        }

        public static ScheduleKey CreateScheduleKey(Guid scheduleId, Guid aggregateId, string description)
        {
            return new ScheduleKey(scheduleId,
                                   $"{typeof(TAggregate).Name}_{aggregateId}_future_event_{scheduleId}",
                                   $"{typeof(TAggregate).Name}_futureEvents",
                                   "");
        }

        public void Handle(FutureEventCanceledEvent futureEventCanceledEvent, IMessageMetadata metadata)
        {
            var message = futureEventCanceledEvent;
            var key = CreateScheduleKey(message.FutureEventId, message.SourceId, "");
            var unscheduleMessage = new Unschedule(key);
            _schedulerActorRef.Tell(unscheduleMessage);
        }
    }

    internal class AggregateExecutionException : Exception
    {
        public AggregateExecutionException(Exception exception) : base("Exception was raised during execution of continuation in aggregate method", exception) {}
    }

    internal class EventApplyException : Exception
    {
        public EventApplyException(Exception exception): base("An error occured while applying event to aggregate. State can be corrupted", exception) {}
    }

    internal class UnknownMessageReceivedException : Exception {}

    internal class UnknownProjectionFinishedException : Exception {}

    internal class UncommitedStateExeption : Exception {}
}