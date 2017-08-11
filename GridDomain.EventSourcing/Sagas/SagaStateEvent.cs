using System;
using GridDomain.Common;

namespace GridDomain.EventSourcing.Sagas
{
    public class SagaStateEvent : DomainEvent
    {
        public SagaStateEvent(Guid processId) : base(processId, processId, createdTime: BusinessDateTime.UtcNow) {}
    }
}