using System;

namespace GridDomain.EventSourcing.Sagas.FutureEvents
{
    public class FutureEventScheduledEvent : DomainEvent
    {
        public Guid Id { get; }
        public DateTime RaiseTime { get;}
        public DomainEvent Event { get;}

        public FutureEventScheduledEvent(Guid id, Guid sourceId, DateTime raiseTime, DomainEvent @event, DateTime? createdTime = null, Guid sagaId = new Guid()) : base(sourceId, createdTime, sagaId)
        {
            Id = id;
            RaiseTime = raiseTime;
            Event = @event;
        }
    }
}