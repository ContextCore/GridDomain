using System;
using System.Collections;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Akka;
using Akka.Actor;
using Akka.Monitoring;
using Akka.Monitoring.Impl;
using CommonDomain.Core;
using CommonDomain.Persistence;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging;
using GridDomain.CQRS.Messaging.Akka;
using GridDomain.CQRS.Messaging.MessageRouting;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.FutureEvents;
using GridDomain.Logging;
using GridDomain.Scheduling.Akka.Messages;
using Helios.Util;

namespace GridDomain.Node.Actors
{
    //TODO: extract non-actor handler to reuse in tests for aggregate reaction for command
    /// <summary>
    ///     Name should be parse by AggregateActorName
    /// </summary>
    /// <typeparam name="TAggregate"></typeparam>
    public class AggregateActor<TAggregate> : EventSourcedActor<TAggregate> where TAggregate : AggregateBase
    {
        private readonly IAggregateCommandsHandler<TAggregate> _handler;
        private readonly TypedMessageActor<ScheduleCommand> _schedulerActorRef;
        private readonly TypedMessageActor<Unschedule> _unscheduleActorRef;

        public const string CreatedFault = "created fault";
        public const string CommandRaisedAnError = "command raised an error";
        public const string PublishingEvent = "Publishing event";
        public const string CommandExecutionCreatedAnEvent = "Command execution created an event";


        public AggregateActor(IAggregateCommandsHandler<TAggregate> handler,
                              TypedMessageActor<ScheduleCommand> schedulerActorRef,
                              TypedMessageActor<Unschedule> unscheduleActorRef,
                              IPublisher publisher,
                              ISnapshotsPersistencePolicy snapshotsPersistencePolicy,
                              IConstructAggregates aggregateConstructor) : base(
                                                                            aggregateConstructor,
                                                                            snapshotsPersistencePolicy,
                                                                            publisher)
        {
            _schedulerActorRef = schedulerActorRef;
            _unscheduleActorRef = unscheduleActorRef;
            _handler = handler;

            //async aggregate method execution finished, aggregate already raised events
            //need process it in usual way
            Command<IMessageMetadataEnvelop<AsyncEventsReceived>>(d =>
            {
                var m = d.Message;
                Monitor.IncrementMessagesReceived();
                if (m.Exception != null)
                {
                    ProcessFault(m.Command, m.Exception, d.Metadata);
                    return;
                }

                (State as Aggregate).FinishAsyncExecution(m.InvocationId);
                ProcessAggregateEvents(m.Command, d.Metadata);
            });

            Command<IMessageMetadataEnvelop<ICommand>>(m =>
            {
                var cmd = m.Message;
                Monitor.IncrementMessagesReceived();
                _log.Trace("{Aggregate} received a {@command}", State.Id, cmd);
                try
                {
                    State = _handler.Execute((TAggregate)State, cmd);
                }
                catch (Exception ex)
                {
                    ProcessFault(cmd, ex, m.Metadata);
                    return;
                }

                ProcessAggregateEvents(cmd, m.Metadata);
            });
        }

        private void ProcessFault(ICommand cmd, Exception ex, IMessageMetadata messageMetadata)
        {
            var fault = Fault.NewGeneric(cmd, ex, typeof(TAggregate), cmd.SagaId);
            
            var metadata = messageMetadata.CreateChild(cmd.Id,
                                           new ProcessEntry(Self.Path.Name,
                                                            CreatedFault,
                                                            CommandRaisedAnError));

            Publisher.Publish(fault, metadata);
            Log.Error(ex, "{Aggregate} raised an expection {@Exception} while executing {@Command}", State.Id, ex, cmd);
        }


        private void ProcessAggregateEvents(ICommand command, IMessageMetadata metadata)
        {
            var events = ExecuteCommand(command);

            int totalEvents = events.Length;
            int persistedEvents = 0;

            PersistAll(events, e =>
            {
                var eventMetadata = metadata.CreateChild(e.SourceId,
                                                         new ProcessEntry(Self.Path.Name,
                                                                          PublishingEvent,
                                                                          CommandExecutionCreatedAnEvent));

                //TODO: move scheduling event processing to some separate handler or aggregateActor extension. 
                //how to pass aggregate type in this case? 
                //direct call to method to not postpone process of event scheduling, 
                //case it can be interrupted by other messages in stash processing errors
                e.Match().With<FutureEventScheduledEvent>(m => Handle(m, eventMetadata))
                         .With<FutureEventCanceledEvent>(m => Handle(m, eventMetadata));

                Publisher.Publish(e, eventMetadata);
                //should save snapshot only after all messages persisted as state was already modified by all of them
                if(++persistedEvents == totalEvents)
                    TrySaveSnapshot(events);

                NotifyWatchers(new Persisted(e));
            });

            State.ClearUncommittedEvents();

            ProcessAsyncMethods(command, metadata);
        }

        private DomainEvent[] ExecuteCommand(ICommand command)
        {
            var events = State.GetUncommittedEvents().Cast<DomainEvent>().ToArray();

            if (command.SagaId != Guid.Empty)
            {
                events = events.Select(e => e.CloneWithSaga(command.SagaId)).ToArray();
            }
            return events;
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

                asyncMethod.ResultProducer.ContinueWith(
                    t =>
                    {
                        var asyncEventsReceived = new AsyncEventsReceived(t.IsFaulted ? null : t.Result, 
                                                                          cmd, 
                                                                          asyncMethod.InvocationId,
                                                                          t.Exception);

                        return new MessageMetadataEnvelop<AsyncEventsReceived>(asyncEventsReceived, metadata);
                    })
                    .PipeTo(Self);
            }

            extendedAggregate.ClearAsyncUncomittedEvents();
        }

        public void Handle(FutureEventScheduledEvent futureEventScheduledEvent, IMessageMetadata messageMetadata)
        {
            var message = futureEventScheduledEvent;
            Guid scheduleId = message.Id;
            Guid aggregateId = message.Event.SourceId;

            var description = $"Aggregate {typeof(TAggregate).Name} id = {aggregateId} scheduled future event " +
                              $"{scheduleId} with payload type {message.Event.GetType().Name} on time {message.RaiseTime}\r\n" +
                              $"Future event: {message.ToPropsString()}";

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

            _schedulerActorRef.Handle(scheduleEvent);
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
            _unscheduleActorRef.Handle(unscheduleMessage);
        }
    }
}