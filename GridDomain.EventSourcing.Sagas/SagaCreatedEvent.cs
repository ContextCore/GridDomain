using System;

namespace GridDomain.EventSourcing.Sagas
{
    public class SagaCreatedEvent<TState> : DomainEvent
    {
        public SagaCreatedEvent(TState state, Guid sourceId, DateTime? createdTime = null) : base(sourceId, createdTime)
        {
            State = state;
        }

        public TState State { get; }
    }
}