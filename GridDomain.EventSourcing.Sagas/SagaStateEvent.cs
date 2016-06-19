using System;

namespace GridDomain.EventSourcing.Sagas
{
    public class SagaStateEvent : DomainEvent
    {
        public SagaStateEvent(Guid sagaId) : base(sagaId, DateTime.UtcNow, sagaId)
        {

        }
    }
}