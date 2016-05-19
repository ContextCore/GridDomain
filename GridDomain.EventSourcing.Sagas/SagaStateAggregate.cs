using System;
using CommonDomain.Core;

namespace GridDomain.EventSourcing.Sagas
{
    public class SagaStateAggregate<TState,TTransition>: AggregateBase
        where TState : struct
        where TTransition : struct
    {
        public TState MachineState;

        private SagaStateAggregate(Guid id)
        {
            Id = id;
        }

        public SagaStateAggregate(Guid id, TState state)
        {
            RaiseEvent(new SagaCreatedEvent<TState>(state,id));
        }

        private void Apply(SagaCreatedEvent<TState> e)
        {
            MachineState = e.State;
            Id = e.SourceId;
        }

        private void Apply(SagaTransitionEvent<TState, TTransition> e)
        {
            MachineState = e.State; 
        }

        public void StateChanged(TTransition t, TState newState)
        {
            RaiseEvent(new SagaTransitionEvent<TState, TTransition>(t,newState,Id));
        }
    }
}