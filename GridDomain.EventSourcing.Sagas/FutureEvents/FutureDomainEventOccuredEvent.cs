using System;

namespace GridDomain.EventSourcing.Sagas.FutureEvents
{
    public class FutureDomainEventOccuredEvent : DomainEvent
    {
        public  Guid OccuranceId { get; }
        public Guid EventId { get; set; }

        public FutureDomainEventOccuredEvent(Guid id, Guid eventId, Guid sourceId)
            : base(sourceId)
        {
            OccuranceId = id;
            EventId = eventId;
        }
    }
}