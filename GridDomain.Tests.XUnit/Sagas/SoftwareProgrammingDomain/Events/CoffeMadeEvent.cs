using System;
using GridDomain.EventSourcing;

namespace GridDomain.Tests.XUnit.Sagas.SoftwareProgrammingDomain.Events
{
    public class CoffeMadeEvent :DomainEvent
    {
        public Guid CoffeMachineId  => SourceId;
        public Guid ForPersonId { get; }
        public CoffeMadeEvent(Guid sourceId, Guid forPersonId, DateTime? createdTime = null, Guid sagaId = new Guid()) : base(sourceId, createdTime, sagaId)
        {
            ForPersonId = forPersonId;
        }
    }
}
