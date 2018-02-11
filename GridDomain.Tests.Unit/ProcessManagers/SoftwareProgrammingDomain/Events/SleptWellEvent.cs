using System;
using GridDomain.EventSourcing;

namespace GridDomain.Tests.Unit.ProcessManagers.SoftwareProgrammingDomain.Events
{
    public class SleptWellEvent : DomainEvent
    {
        public SleptWellEvent(string sourceId, string sofaId, string processId = null, DateTime? createdTime = null)
            : base(sourceId, processId, createdTime: createdTime)
        {
            SofaId = sofaId;
        }

        public string SofaId { get; }
        public string PersonId => SourceId;
    }
}