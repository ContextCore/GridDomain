using System;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;
using GridDomain.Common;
using GridDomain.EventSourcing.VersionedTypeSerialization;

namespace GridDomain.EventSourcing
{
    //[Serializable]
    public class DomainEvent : ISourcedEvent//, ISerializable
    {
        public DomainEvent(Guid sourceId, DateTime? createdTime = null, Guid sagaId = default(Guid))
        {
            SourceId = sourceId;
            CreatedTime = createdTime ?? DateTimeFacade.UtcNow;
            SagaId = sagaId;
        }

        //Source of the event - aggregate that created it
        public Guid SourceId { get;}
        public Guid SagaId { get; protected set; }
        public DateTime CreatedTime { get; }

        public DomainEvent CloneWithSaga(Guid sagaId)
        {
            var evt = (DomainEvent) MemberwiseClone();
            evt.SagaId = sagaId;
            return evt;
        }

        //relying on external serializer to pass all SerializationInfo already filled as a parameter
        //[SecurityPermission(SecurityAction.LinkDemand,Flags = SecurityPermissionFlag.SerializationFormatter)]
        //void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        //{
        //if (info == null)
        //throw new System.ArgumentNullException(nameof(info));
        //var versionedTypeName = VersionedTypeName.Parse(info.FullTypeName,this.Version);
        //info.FullTypeName = versionedTypeName.ToString();

        //}
    }
}