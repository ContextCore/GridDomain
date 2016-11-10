using System;
using CommonDomain.Core;

namespace GridDomain.EventSourcing.Sagas.StateSagas
{
    //[Obsolete("Use Saga classes instead")]
    public class SagaStateAggregate<TState, TTransition> : AggregateBase
                                                             where TState : struct
                                                             where TTransition : struct
    {
        public TState MachineState { get; private set; }

        protected SagaStateAggregate(Guid id)
        {
            Id = id;
        }

        public void RaiseEvent(DomainEvent e)
        {
            base.RaiseEvent(e.CloneWithSaga(Id));
        }

        public SagaStateAggregate(Guid id, TState machineState):this(id)
        {
            RaiseEvent(new SagaCreatedEvent<TState>(machineState, id));
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