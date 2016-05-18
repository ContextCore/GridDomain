using System;
using System.Collections.Generic;
using CommonDomain.Core;

namespace GridDomain.CQRS.Messaging.MessageRouting.Sagas
{
    public class SagaStateAggregate<TState,TTransition>: AggregateBase
        where TState : struct
        where TTransition : struct
    {
        public TState State;


        public class StateHistory
        {
            public TTransition Transition;
           // public TState From;
            public TState To;
        }

        private SagaStateAggregate(Guid id)
        {
            Id = id;
        }

        public SagaStateAggregate(Guid id, TState state)
        {
            Apply(new SagaCreatedEvent<TState>(state,id));
        }

        private void Apply(SagaCreatedEvent<TState> e)
        {
            State = e.State;
        }

        private void Apply(SagaTransitionEvent<TState, TTransition> e)
        {
            State = e.State; 
        }

        public void Transit(TTransition t, TState newState)
        {
            RaiseEvent(new SagaTransitionEvent<TState, TTransition>(t,newState,Id));
        }
    }
}