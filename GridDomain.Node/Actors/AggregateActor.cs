using System;
using System.Linq;
using Akka;
using Akka.Actor;
using CommonDomain;
using CommonDomain.Core;
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
    //TODO: extract non-actor handler to reuse in tests for aggregate reaction for command
    /// <summary>
    ///     Name should be parse by AggregateActorName
    /// </summary>
    /// <typeparam name="TAggregate"></typeparam>
    public class AggregateActor<TAggregate> : EventSourcedActor<TAggregate> where TAggregate : AggregateBase
    {
        public const string CreatedFault = "created fault";
        public const string CommandRaisedAnError = "command raised an error";
        public const string PublishingEvent = "Publishing event";
        public const string CommandExecutionCreatedAnEvent = "Command execution created an event";
        private readonly IActorRef _customHandlersActor;
        private readonly ProcessEntry _domainEventProcessEntry;
        private readonly ProcessEntry _domainEventProcessFailEntry;
        private readonly IAggregateCommandsHandler<TAggregate> _handler;
        private readonly IPublisher _publisher;
        private readonly IActorRef _schedulerActorRef;

        public AggregateActor(IAggregateCommandsHandler<TAggregate> handler,
                              IActorRef schedulerActorRef,
                              IPublisher publisher,
                              ISnapshotsPersistencePolicy snapshotsPersistencePolicy,
                              IConstructAggregates aggregateConstructor,
                              IActorRef customHandlersActor) : base(aggregateConstructor, snapshotsPersistencePolicy)
        {
            _publisher = publisher;
            _customHandlersActor = customHandlersActor;
            _schedulerActorRef = schedulerActorRef;
            _handler = handler;
            _domainEventProcessEntry = new ProcessEntry(Self.Path.Name, PublishingEvent, CommandExecutionCreatedAnEvent);

            _domainEventProcessFailEntry = new ProcessEntry(Self.Path.Name, CreatedFault, CommandRaisedAnError);

            Command<IMessageMetadataEnvelop<ICommand>>(m =>
                                                       {
                                                           var cmd = m.Message;
                                                           Monitor.IncrementMessagesReceived();
                                                           Log.Debug("{Aggregate} received a {@command}", PersistenceId, cmd);

                                                           _handler.ExecuteAsync((TAggregate) State, cmd)
                                                                   .PipeTo(Self);

                                                           BecomeStacked(() => WaitingForCommandExecution(m));
                                                       });
        }

        private void WaitingForCommandExecution(IMessageMetadataEnvelop<ICommand> commandEnvelop)
        {
            CommandAny(c =>
                       {
                           c.Match()
                            .With<Failure>(f =>
                                           {
                                               CommandError(commandEnvelop, f);

                                               UnbecomeStacked();
                                               BecomeStacked(() => WaitingHandlersProcess(commandEnvelop.Metadata));
                                           })
                               // .With<Status.Failure>()
                            .With<IAggregate>(newState =>
                                              {
                                                  CommandSuccess(commandEnvelop, newState);

                                                  UnbecomeStacked();
                                                  BecomeStacked(() => WaitingHandlersProcess(commandEnvelop.Metadata));
                                                  Stash.UnstashAll();
                                              })
                            .Default(o => Stash.Stash());
                       });
        }

        private void CommandError(IMessageMetadataEnvelop<ICommand> commandEnvelop, Failure f)
        {
            WaitForFaultProcessedByHandlers(commandEnvelop.Message, f.Exception, commandEnvelop.Metadata);
        }

        private void CommandSuccess(IMessageMetadataEnvelop<ICommand> commandEnvelop, IAggregate newState)
        {
            State = newState;

            var uncommittedEvents =
                State.GetUncommittedEvents()
                     .Cast<DomainEvent>()
                     .Select(e => e.CloneWithSaga(commandEnvelop.Message.SagaId))
                     .ToArray();

            State.ClearUncommittedEvents();
            var eventsMetadata = commandEnvelop.Metadata.CreateChild(Id, _domainEventProcessEntry);
            OnCommandEventsPersisted(uncommittedEvents, eventsMetadata);
           
        }

        protected override void Terminating()
        {
            Command<IMessageMetadataEnvelop<ICommand>>(c =>
                                                       {
                                                           Self.Tell(CancelShutdownRequest.Instance);
                                                           Stash.Stash();
                                                       });
            base.Terminating();
        }

        private void WaitForFaultProcessedByHandlers(ICommand cmd, Exception ex, IMessageMetadata messageMetadata)
        {
            var fault = Fault.NewGeneric(cmd, ex, cmd.SagaId, typeof(TAggregate));

            var metadata = messageMetadata.CreateChild(cmd.Id, _domainEventProcessFailEntry);

            _customHandlersActor.Ask<AllHandlersCompleted>(new MessageMetadataEnvelop<IFault>(fault, metadata)).PipeTo(Self);

            Log.Error(ex, "{Aggregate} raised an error {@Exception} while executing {@Command}", PersistenceId, ex, cmd);
        }

        //private void PersistState(ICommand command, IMessageMetadata commandMetadata)
        //{
        //    var events =
        //        State.GetUncommittedEvents().Cast<DomainEvent>().Select(e => e.CloneWithSaga(command.SagaId)).ToArray();
        //
        //    var totalEvents = events.Length;
        //    var persistedEvents = 0;
        //    var eventsMetadata = commandMetadata.CreateChild(Id, _domainEventProcessEntry);
        //
        //    PersistAll(events,
        //        e =>
        //        {
        //         
        //        });
        //
        //    State.ClearUncommittedEvents();
        //}
        

        private void WaitingHandlersProcess(IMessageMetadata commandMetadata)
        {
            CommandAny(c =>{
                            c.Match()
                             .With<AllHandlersCompleted>(m =>
                                                         {
                                                             OnHandlersFinish(commandMetadata, m);

                                                             UnbecomeStacked();
                                                             Stash.UnstashAll();
                                                         })
                            //  .With<IMessageMetadataEnvelop<AsyncEventsReceived>>(d => { FinishAsyncInvoke(d); })
                             .Default(o => Stash.Stash());
                     });
        }

       // private void FinishAsyncInvoke(IMessageMetadataEnvelop<AsyncEventsReceived> d)
       // {
       //     var m = d.Message;
       //     Monitor.IncrementMessagesReceived();
       //     if (m.Exception != null)
       //     {
       //         WaitForFaultProcessedByHandlers(m.Command, m.Exception, d.Metadata);
       //         return;
       //     }
       //
       //     (State as Aggregate).FinishAsyncExecution(m.InvocationId);
       //     //PersistState(m.Command, d.Metadata);
       // }

        private void OnHandlersFinish(IMessageMetadata commandMetadata, AllHandlersCompleted processComplete)
        {
            foreach (var e in processComplete.DomainEvents)
            {
                var eventMetadata = commandMetadata.CreateChild(e.SourceId, _domainEventProcessEntry);
                _publisher.Publish(e, eventMetadata);
            }

            if (processComplete.Fault == null) return;

            var faultMetadata = commandMetadata.CreateChild(commandMetadata.MessageId, _domainEventProcessFailEntry);
            _publisher.Publish(processComplete.Fault, faultMetadata);
        }

        protected override void OnEventPersisted(DomainEvent[] events)
        {
            foreach (var e in events)
                NotifyPersistenceWatchers(e);
        }

        private void OnCommandEventsPersisted(DomainEvent[] events, IMessageMetadata eventCommonMetadata)
        {
            TrySaveSnapshot();

            var envelop = new MessageMetadataEnvelop<DomainEvent[]>(events, eventCommonMetadata);

            foreach (var e in events)
            {
                //TODO: move scheduling event processing to some separate handler or aggregateActor extension. 
                //how to pass aggregate type in this case? 
                //direct call to method to not postpone process of event scheduling, 
                //case it can be interrupted by other messages in stash processing errors
                e.Match()
                 .With<FutureEventScheduledEvent>(m => Handle(m, eventCommonMetadata))
                 .With<FutureEventCanceledEvent>(m => Handle(m, eventCommonMetadata));
            }

            _customHandlersActor.Ask<AllHandlersCompleted>(envelop).PipeTo(Self);
        }

        private void ProcessAsyncMethods(ICommand command, IMessageMetadata metadata)
        {
            var extendedAggregate = State as Aggregate;
            if (extendedAggregate == null) return;

            //When aggregate notifies external world about async method execution start,
            //actor should schedule results to process it
            //command is included to safe access later, after async execution complete
            var cmd = command;
            foreach (var asyncMethod in extendedAggregate.GetAsyncUncomittedEvents())
            {
                asyncMethod.ResultProducer.ContinueWith(t =>
                                                        {
                                                            var asyncEventsReceived =
                                                                new AsyncEventsReceived(t.IsFaulted ? null : t.Result,
                                                                    cmd,
                                                                    asyncMethod.InvocationId,
                                                                    t.Exception);

                                                            return
                                                                new MessageMetadataEnvelop<AsyncEventsReceived>(
                                                                    asyncEventsReceived,
                                                                    metadata);
                                                        }).PipeTo(Self);
            }
            extendedAggregate.ClearAsyncUncomittedEvents();
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
}