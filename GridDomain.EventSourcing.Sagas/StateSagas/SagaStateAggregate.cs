using System;
using CommonDomain.Core;

namespace GridDomain.EventSourcing.Sagas
{
    [Obsolete("Use Saga classes instead")]
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
            base.RaiseEvent(@event);
        }

        
        public SagaStateAggregate(Guid id, TState state)
        {
            RaiseEvent(new SagaCreatedEvent<TState>(state, id).CloneWithSaga(id));
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
            RaiseEvent(new SagaTransitionEvent<TState, TTransition>(t, newState, Id).CloneWithSaga(Id));
        }
    }
}