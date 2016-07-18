using System;
using GridDomain.EventSourcing;

namespace GridDomain.Tests.Sagas.InstanceSagas.Events
{
    public class CoffeMadeDomainEvent :DomainEvent
    {
        public Guid CoffeMachineId  => SourceId;
        public Guid ForPersonId { get; }
        public CoffeMadeDomainEvent(Guid sourceId, Guid forPersonId, DateTime? createdTime = null, Guid sagaId = new Guid()) : base(sourceId, createdTime, sagaId)
        {
            ForPersonId = forPersonId;
        }
    }
}
