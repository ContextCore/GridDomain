using System;
using System.Threading.Tasks;
using Akka.Actor;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.Logging;
using GridDomain.Scheduling.Akka.Messages;

namespace GridDomain.Scheduling.FutureEvents
{
    /// <summary>
    ///     Listening to scheduling events from aggregates and modify quartz jobs accordinally
    /// </summary>
    internal class FutureEventsShedulingMessageHandler : IHandler<FutureEventScheduledEvent>,
                                                         IHandler<FutureEventCanceledEvent>
    {
        private readonly IActorRef _schedulerActorRef;
        private readonly ProcessEntry _schedulingFutureEventProcessEntry;

        public FutureEventsShedulingMessageHandler(IActorRef schedulingActor)
        {
            _schedulerActorRef = schedulingActor;

            _schedulingFutureEventProcessEntry = new ProcessEntry(GetType().
                    Name,
                "Scheduling raise future event command",
                "FutureEventScheduled event occured");
        }

        public Task Handle(FutureEventCanceledEvent evt, IMessageMetadata metadata)
        {
            var key = CreateScheduleKey(evt.FutureEventId, evt.SourceId);
            return _schedulerActorRef.Ask<Unscheduled>(new Unschedule(key));
        }

        public Task Handle(FutureEventScheduledEvent message, IMessageMetadata messageMetadata)
        {
            var scheduleId = message.Id;
            var succesEventType = message.Event.GetType();

            var description = $"Source {message.SourceName} scheduled future event "
                              + $"{scheduleId} with payload type {succesEventType. Name} on time {message.RaiseTime}\r\n"
                              + $"Future event: {message.ToPropsString()}";

            var scheduleKey = CreateScheduleKey(scheduleId, message.SourceId, message.SourceName, description);

            var command = new RaiseScheduledDomainEventCommand(message.Id, message.SourceId, Guid.NewGuid());
            var metadata = messageMetadata.CreateChild(command.Id,_schedulingFutureEventProcessEntry);

            var scheduleEvent = new ScheduleCommandExecution(command,
                                                    scheduleKey,
                                                    ExecutionOptions.ForCommand(message.RaiseTime, succesEventType),
                                                    metadata);

            return _schedulerActorRef.Ask<Scheduled>(scheduleEvent);
        }

        internal static ScheduleKey CreateScheduleKey(Guid scheduleId, Guid sourceId, string sourceName=null, string description=null)
        {
            return new ScheduleKey(scheduleId,
                $"{sourceName}_{sourceId}_future_event_{scheduleId}",
                $"{sourceName}_{sourceId}_futureEvents",
                description);
        }
    }
}