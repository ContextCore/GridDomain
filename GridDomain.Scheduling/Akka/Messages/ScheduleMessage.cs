using System;
using GridDomain.Common;
using GridDomain.EventSourcing;

namespace GridDomain.Scheduling.Akka.Messages
{
    public class ScheduleMessage
    {
        public DomainEvent Event { get; }
        public ScheduleKey Key { get; }
        public DateTime RunAt { get; }
        public IMessageMetadata EventMetadata { get; }

        public ScheduleMessage(DomainEvent eventToSchedule, ScheduleKey key, DateTime runAt, IMessageMetadata eventMetadata = null)
        {
            Event = eventToSchedule;
            Key = key;
            RunAt = runAt;
            EventMetadata = eventMetadata;
        }
    }
}