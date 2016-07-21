using System;

namespace GridDomain.EventSourcing.Sagas.FutureEvents
{
    public class FutureEventOccuredEvent : DomainEvent
    {
        public Guid Id { get; set; }
        public Guid FutureEventId { get;}

        public FutureEventOccuredEvent(Guid id, Guid futureEventId, Guid sourceId)
            : base(sourceId)
        {
            Id = id;
            FutureEventId = futureEventId;
        }
    }
}