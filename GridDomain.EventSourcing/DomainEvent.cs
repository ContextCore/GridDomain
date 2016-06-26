using System;
using System.Linq;
using CommonDomain;

namespace GridDomain.EventSourcing
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

    public class DomainEvent : ISourcedEvent
    {
        public DomainEvent(Guid sourceId, DateTime? createdTime = null, Guid sagaId = default(Guid))
        {
            SourceId = sourceId;
            CreatedTime = createdTime ?? DateTime.UtcNow;
            SagaId = sagaId;
        }

        //Source of the event - aggregate that created it
        public Guid SourceId { get;}
        //ensure sagaId will not be changed in actors
        public Guid SagaId { get; private set; }
        public DateTime CreatedTime { get; }
        public virtual int Version { get; } = 1;

        public DomainEvent CloneWithSaga(Guid sagaId)
        {
            var evt = (DomainEvent) MemberwiseClone();
            evt.SagaId = sagaId;
            return evt;
        }
    }
}