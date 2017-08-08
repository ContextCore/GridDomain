using System;

namespace GridDomain.Scheduling
{
    public class ScheduledEventNotFoundException : Exception
    {
        public ScheduledEventNotFoundException(Guid eventId)
        {
            EventId = eventId;
        }

        public Guid EventId { get; }
    }
}