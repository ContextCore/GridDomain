using System;

namespace GridDomain.EventSourcing.Sagas.FutureEvents
{
    public class FutureDomainEventOccuredEvent : DomainEvent
    {
        public Guid Id { get; set; }
        public Guid FutureEventId { get;}

        public FutureDomainEventOccuredEvent(Guid id, Guid futureEventId, Guid sourceId)
            : base(sourceId)
        {
            Id = id;
            FutureEventId = futureEventId;
        }
    }
}