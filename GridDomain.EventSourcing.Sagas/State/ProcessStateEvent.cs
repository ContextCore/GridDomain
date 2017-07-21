using System;
using GridDomain.Common;
using GridDomain.EventSourcing;

namespace GridDomain.Processes.State
{
    public class ProcessStateEvent : DomainEvent
    {
        public ProcessStateEvent(Guid processId) : base(processId, processId: processId, createdTime: BusinessDateTime.UtcNow) {}
    }
}