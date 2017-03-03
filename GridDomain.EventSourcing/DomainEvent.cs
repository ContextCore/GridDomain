using System;
using GridDomain.Common;

namespace GridDomain.EventSourcing
{
    public class DomainEvent : ISourcedEvent, IHaveId
    {
        public DomainEvent(Guid sourceId, DateTime? createdTime = null, Guid? sagaId = null)
        {
            SourceId = sourceId;
            CreatedTime = createdTime ?? BusinessDateTime.UtcNow;
            SagaId = sagaId ?? Guid.NewGuid();
        }

        //Source of the event - aggregate that created it
        // private setter for serializers
        public Guid SourceId { get; }
        public Guid SagaId { get; private set; }
        public DateTime CreatedTime { get; }
        public Guid Id => SourceId;


        public DomainEvent CloneWithSaga(Guid sagaId)
        {
            var evt = (DomainEvent) MemberwiseClone();
            evt.SagaId = sagaId;
            return evt;
        }

    }
}