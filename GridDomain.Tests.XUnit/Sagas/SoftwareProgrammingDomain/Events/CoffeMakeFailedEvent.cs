using System;
using GridDomain.EventSourcing;

namespace GridDomain.Tests.XUnit.Sagas.SoftwareProgrammingDomain.Events
{
    public class CoffeMakeFailedEvent :DomainEvent
    {
        public Guid CoffeMachineId => SourceId;
        public Guid ForPersonId { get; }
        public CoffeMakeFailedEvent(Guid sourceId, Guid forPersonId, DateTime? createdTime = null, Guid sagaId = new Guid()) : base(sourceId, createdTime, sagaId)
        {
            ForPersonId = forPersonId;
        }
    }
}