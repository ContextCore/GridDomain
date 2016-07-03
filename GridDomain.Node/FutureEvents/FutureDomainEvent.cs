using System;
using GridDomain.EventSourcing;

namespace GridDomain.Node.FutureEvents
{
    public class FutureDomainEvent : DomainEvent
    {
        public DateTime RaiseTime { get; set; }
        public DomainEvent Event { get; set; }

        public FutureDomainEvent(Guid sourceId, DateTime raiseTime, DomainEvent @event, DateTime? createdTime = null, Guid sagaId = new Guid()) : base(sourceId, createdTime, sagaId)
        {
            RaiseTime = raiseTime;
            Event = @event;
        }
    }
}