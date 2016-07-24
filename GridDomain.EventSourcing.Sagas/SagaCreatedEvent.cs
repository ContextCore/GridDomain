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


    public class InstanceSagaCreatedEvent<TData> : SagaStateEvent
    {
        public string StateName { get; }

        public InstanceSagaCreatedEvent(TData data, Guid sourceId, string stateName) : base(sourceId)
        {
            Data = data;
            StateName = stateName;
        }

        public InstanceSagaCreatedEvent(TData data, Guid sourceId, State state) 
            : this(data,sourceId,state.Name)
        {
        }

        public TData Data { get; }
    }
}