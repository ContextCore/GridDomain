using System;

namespace GridDomain.EventSourcing.FutureEvents
{
    public class FutureEventOccuredEvent : DomainEvent
    {
        public FutureEventOccuredEvent(Guid id, Guid futureEventId, Guid sourceId) : base(sourceId)
        {
            Id = id;
            FutureEventId = futureEventId;
        }

        public new Guid Id { get; }
        public Guid FutureEventId { get; }
    }
}