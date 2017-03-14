using System;
using GridDomain.Common;

namespace GridDomain.EventSourcing.Sagas
{
    public class SagaStateEvent : DomainEvent
    {
        public SagaStateEvent(Guid sagaId) : base(sagaId, sagaId: sagaId, createdTime: BusinessDateTime.UtcNow) {}
    }
}