using System;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Serialization;
using System.Security.Permissions;
using GridDomain.Common;
using GridDomain.EventSourcing.VersionedTypeSerialization;

namespace GridDomain.EventSourcing
{
  

    public class DomainEvent : ISourcedEvent
    {
        public DomainEvent(Guid sourceId, DateTime? createdTime = null, Guid? sagaId=null)
        {
            SourceId = sourceId;
            CreatedTime = createdTime ?? BusinessDateTime.UtcNow;
            SagaId = sagaId ?? Guid.NewGuid();
        }

        //Source of the event - aggregate that created it
        // private setter for serializers
        public Guid SourceId { get; private set; }
        public Guid SagaId { get; private set; }
        public DateTime CreatedTime { get; private set; }

        public DomainEvent CloneWithSaga(Guid sagaId)
        {
            var evt = (DomainEvent) MemberwiseClone();
            evt.SagaId = sagaId;
            return evt;
        }
    }
}