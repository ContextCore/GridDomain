using System;
using GridDomain.Common;
using GridDomain.EventSourcing;

namespace GridDomain.ProcessManagers.State
{
    public class ProcessStateEvent : DomainEvent
    {
        public ProcessStateEvent(string processId) : base(processId, processId: processId, createdTime: BusinessDateTime.UtcNow) {}
    }
}