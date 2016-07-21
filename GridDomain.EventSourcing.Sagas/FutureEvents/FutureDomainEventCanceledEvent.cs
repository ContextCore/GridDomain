using System;

namespace GridDomain.EventSourcing.Sagas.FutureEvents
{
    public class FutureDomainEventCanceledEvent : DomainEvent
    {
        public Guid FutureEventId { get;}

        public FutureDomainEventCanceledEvent(Guid futureEventId, Guid sourceId, DateTime? createdTime = null, Guid sagaId = new Guid()) : base(sourceId, createdTime, sagaId)
        {
            FutureEventId = futureEventId;
        }
    }
}