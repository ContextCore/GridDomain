using System;

namespace GridDomain.EventSourcing.FutureEvents
{
    public class ScheduledEventNotFoundException : Exception
    {
        public Guid EventId { get; set; }

        public ScheduledEventNotFoundException(Guid eventId)
        {
            EventId = eventId;
        }
    }
}