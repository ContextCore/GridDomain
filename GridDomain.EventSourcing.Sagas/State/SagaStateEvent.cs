using System;
using GridDomain.Common;
using GridDomain.EventSourcing;

namespace GridDomain.Processes.State
{
    public class SagaStateEvent : DomainEvent
    {
        public SagaStateEvent(Guid processId) : base(processId, processId: processId, createdTime: BusinessDateTime.UtcNow) {}
    }
}