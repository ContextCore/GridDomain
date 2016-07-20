using System;

namespace GridDomain.EventSourcing.Sagas.FutureEvents
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