using System;
using GridDomain.EventSourcing;

namespace GridDomain.Scheduling.FutureEvents
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