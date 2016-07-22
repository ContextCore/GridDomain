using System;

namespace GridDomain.EventSourcing.Sagas.FutureEvents
{
    public class FutureEventCanceledEvent : DomainEvent
    {
        public Guid FutureEventId { get;}

        public FutureEventCanceledEvent(Guid futureEventId, Guid sourceId, DateTime? createdTime = null, Guid sagaId = new Guid()) : base(sourceId, createdTime, sagaId)
        {
            FutureEventId = futureEventId;
        }
    }
}