using System;
using System.Collections.Generic;
using CommonDomain.Core;

namespace GridDomain.CQRS.Messaging.MessageRouting.Sagas
{
    public class SagaStateAggregate<TState,TTransition>: AggregateBase
        where TState : struct
        where TTransition : struct
    {
        public TState InitialState;
        public List<TTransition> Transitions = new List<TTransition>();
        public List<object> PendingEvents = new List<object>(); 

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
            InitialState = e.InitialState;
        }

        private void Apply(SagaTransitionEvents<TTransition> e)
        {
            Transitions.Add(e.Transition);
        }
    }
}