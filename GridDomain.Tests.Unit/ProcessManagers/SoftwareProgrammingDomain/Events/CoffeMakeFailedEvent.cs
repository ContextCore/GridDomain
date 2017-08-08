using System;
using GridDomain.EventSourcing;

namespace GridDomain.Tests.Unit.ProcessManagers.SoftwareProgrammingDomain.Events
{
    public class CoffeMakeFailedEvent : DomainEvent
    {
        public CoffeMakeFailedEvent(Guid sourceId, Guid forPersonId, DateTime? createdTime = null, Guid processId = new Guid())
            : base(sourceId,  processId, createdTime: createdTime)
        {
            ForPersonId = forPersonId;
        }

        public Guid CoffeMachineId => SourceId;
        public Guid ForPersonId { get; }
    }
}