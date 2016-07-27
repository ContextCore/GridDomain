using System;
using Automatonymous;

namespace GridDomain.EventSourcing.Sagas
{
    public class SagaCreatedEvent<TState> : SagaStateEvent
    {
        public SagaCreatedEvent(TState state, Guid sourceId) : base(sourceId)
        {
            State = state;
        }

        public TState State { get; }
    }
    
}