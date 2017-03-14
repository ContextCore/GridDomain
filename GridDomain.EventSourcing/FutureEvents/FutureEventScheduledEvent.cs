using System;

namespace GridDomain.EventSourcing.FutureEvents
{
    public class FutureEventScheduledEvent : DomainEvent
    {
        public FutureEventScheduledEvent(Guid id,
                                         Guid sourceId,
                                         DateTime raiseTime,
                                         DomainEvent @event,
                                         DateTime? createdTime = null,
                                         Guid? sagaId = null) : base(sourceId, sagaId, id, createdTime)
        {
            RaiseTime = raiseTime;
            Event = @event;
        }

        public DateTime RaiseTime { get; }
        public DomainEvent Event { get; }
    }
}