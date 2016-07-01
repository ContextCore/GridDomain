using System;
using System.Linq;
using GridDomain.Common;

namespace GridDomain.EventSourcing
{
    public class DomainEvent : ISourcedEvent
    {
        public DomainEvent(Guid sourceId, DateTime? createdTime = null, Guid sagaId = default(Guid))
        {
            SourceId = sourceId;
            CreatedTime = createdTime ?? DateTimeFacade.UtcNow;
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