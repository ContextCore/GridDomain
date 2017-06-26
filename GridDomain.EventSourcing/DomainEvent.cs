using System;
using GridDomain.Common;

namespace GridDomain.EventSourcing
{
    public class DomainEvent : ISourcedEvent, IHaveId, IHaveSagaId
    {
        protected DomainEvent(Guid sourceId, Guid? sagaId = null, Guid? id = null, DateTime? createdTime = null)
        {
            SourceId = sourceId;
            CreatedTime = createdTime ?? BusinessDateTime.UtcNow;
            SagaId = sagaId ?? Guid.Empty;
            Id = id ?? Guid.NewGuid();
        }

        //Source of the event - aggregate that created it
        //private setter for serializers
        public Guid SourceId { get; private set; }
        public Guid SagaId { get; private set; }
        public DateTime CreatedTime { get; private set; }
        public Guid Id { get; private set; }

        public override bool Equals(object obj)
        {
            if (obj!= null && obj is DomainEvent e)
            {
                return e.Id == Id;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public DomainEvent CloneWithSaga(Guid sagaId)
        {
            var evt = (DomainEvent) MemberwiseClone();
            evt.SagaId = sagaId;
            return evt;
        }

    }
}