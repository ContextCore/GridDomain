using System;
using GridDomain.EventSourcing;

namespace GridDomain.Tests.Sagas.InstanceSagas.Events
{
    public class CoffeMakeFailedDomainEvent :DomainEvent
    {
        public Guid CoffeMachineId => SourceId;
        public Guid ForPersonId { get; }
        public CoffeMakeFailedDomainEvent(Guid sourceId, Guid forPersonId, DateTime? createdTime = null, Guid sagaId = new Guid()) : base(sourceId, createdTime, sagaId)
        {
            ForPersonId = forPersonId;
        }
    }
}