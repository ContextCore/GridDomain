using System;
using GridDomain.EventSourcing;

namespace GridDomain.Tests.Unit.ProcessManagers.SoftwareProgrammingDomain.Events
{
    public class SleptWellEvent : DomainEvent
    {
        public SleptWellEvent(Guid sourceId, Guid sofaId, Guid processId = new Guid(), DateTime? createdTime = null)
            : base(sourceId, processId, createdTime: createdTime)
        {
            SofaId = sofaId;
        }

        public Guid SofaId { get; }
        public Guid PersonId => SourceId;
    }
}