using System;
using GridDomain.EventSourcing;

namespace GridDomain.Tests.Unit.ProcessManagers.SoftwareProgrammingDomain.Events
{
    public class CoffeMadeEvent : DomainEvent
    {
        public CoffeMadeEvent(string sourceId, string forPersonId, DateTime? createdTime = null, string processId = null)
            : base(sourceId, processId: processId, createdTime: createdTime)
        {
            ForPersonId = forPersonId;
        }

        public string CoffeMachineId => SourceId;
        public string ForPersonId { get; }
    }
}