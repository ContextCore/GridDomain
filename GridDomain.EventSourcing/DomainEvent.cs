using System;
using GridDomain.Common;

namespace GridDomain.EventSourcing
{
    public class DomainEvent : ISourcedEvent, IHaveId
    {
        public DomainEvent(Guid sourceId, Guid? sagaId = null, Guid? id = null, DateTime? createdTime = null)
        {
            SourceId = sourceId;
            CreatedTime = createdTime ?? BusinessDateTime.UtcNow;
            SagaId = sagaId ?? Guid.NewGuid();
            Id = sagaId ?? Guid.NewGuid();
        }

        //Source of the event - aggregate that created it
        // private setter for serializers
        public Guid SourceId { get; }
        public Guid SagaId { get; private set; }
        public DateTime CreatedTime { get; }
        public Guid Id { get; }


        public DomainEvent CloneWithSaga(Guid sagaId)
        {
            var evt = (DomainEvent) MemberwiseClone();
            evt.SagaId = sagaId;
            return evt;
        }

    }
}