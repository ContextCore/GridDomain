using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Automatonymous;
using CommonDomain.Core;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Sagas;

namespace GridDomain.Tests.Sagas.Simplified
{
    public class SagaProgressAggregate<TState> : AggregateBase, ISagaProgress<TState>
    {
        public TState CurrentState { get; set; }

        protected SagaProgressAggregate(Guid id)
        {
            Id = id;
        }

        public SagaProgressAggregate(Guid id, State state):this(id)
        {
            RaiseEvent(new SagaCreatedEvent<State>(state, id));
        }

        private readonly IDictionary<Type,Action<object>> _messagesAppliers = new Dictionary<Type, Action<object>>(); 

        protected void OnRecieve<TMessage>(Action<TMessage> messageApply)
        {
            _messagesAppliers[typeof(TMessage)] = m => messageApply((TMessage)m);
        }

        public void Apply(SagaCreatedEvent<TState> e)
        {
            CurrentState = e.State;
            Id = e.SourceId;
        }

        public void Apply(SagaTransitionEvent<TState> e)
        {
            CurrentState = e.NewState;
            Action<object> messageApplier;
            if(_messagesAppliers.TryGetValue(e.Message.GetType(), out messageApplier))
                messageApplier.Invoke(e.Message);
        }

        public void StateChanged(object message, TState newState)
        {
            RaiseEvent(new SagaTransitionEvent<TState>(Id, newState, message));
        }


        public class SagaTransitionEvent<TState> : SagaStateEvent
        {
            public object Message { get; }
     
            public TState NewState { get; }

            public SagaTransitionEvent(Guid sourceId, TState newState, object message)
                : base(sourceId)
            {
                NewState = newState;
                Message = message;
            }
        }
    }
}