using System;

namespace GridDomain.EventSourcing.FutureEvents
{
    public class FutureEventCanceledEvent : DomainEvent
    {
        public FutureEventCanceledEvent(Guid futureEventId,
                                        Guid sourceId,
                                        DateTime? createdTime = null,
                                        Guid? sagaId = null) : base(sourceId,  sagaId, createdTime: createdTime)
        {
            FutureEventId = futureEventId;
        }

        public Guid FutureEventId { get; }
    }
}