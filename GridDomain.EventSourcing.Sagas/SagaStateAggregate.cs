using System;
using CommonDomain.Core;

namespace GridDomain.EventSourcing.Sagas
{
    public class SagaStateAggregate<TState, TTransition> : AggregateBase
                                                             where TState : struct
                                                             where TTransition : struct
    {
        public TState MachineState;

        protected SagaStateAggregate(Guid id)
        {
            Id = id;
        }

        protected new void RaiseEvent(DomainEvent @event)
        {
            base.RaiseEvent(@event.CloneWithSaga(Id));
        }

        
        public SagaStateAggregate(Guid id, TState state)
        {
            RaiseEvent(new SagaCreatedEvent<TState>(state, id));
        }

        public void Apply(SagaCreatedEvent<TState> e)
        {
            MachineState = e.State;
            Id = e.SourceId;
        }

        public void Apply(SagaTransitionEvent<TState, TTransition> e)
        {
            MachineState = e.State;
        }

        public void StateChanged(TTransition t, TState newState)
        {
            RaiseEvent(new SagaTransitionEvent<TState, TTransition>(t, newState, Id));
        }
    }
}