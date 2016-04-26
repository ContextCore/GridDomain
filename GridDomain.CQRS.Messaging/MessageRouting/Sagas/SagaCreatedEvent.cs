using System;
using GridDomain.EventSourcing;

namespace GridDomain.CQRS.Messaging.MessageRouting.Sagas
{
    public class SagaCreatedEvent<TState> : DomainEvent
    {
        public SagaCreatedEvent(TState state, Guid sourceId, DateTime? createdTime = null) : base(sourceId, createdTime)
        {
            InitialState = state;
        }

        public TState InitialState { get; }
    }
}