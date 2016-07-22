using System;

namespace GridDomain.EventSourcing.Sagas
{
    public class SagaCreatedEvent<TState> : SagaStateEvent
    {
        public string StateName { get;  }
        public SagaCreatedEvent(TState state, Guid sourceId, string stateName = null) : base(sourceId)
        {
            State = state;
            StateName = stateName;
        }

        public TState State { get; }
    }
}