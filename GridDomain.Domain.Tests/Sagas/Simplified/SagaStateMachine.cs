using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Automatonymous;
using Automatonymous.Contexts;
using GridDomain.CQRS;
using GridDomain.EventSourcing.Sagas;

namespace GridDomain.Tests.Sagas.Simplified
{
    public class SagaStateMachine<TSagaState> : AutomatonymousStateMachine<TSagaState> where TSagaState : class, ISagaProgress
    {
        public readonly List<ICommand> CommandsToDispatch = new List<ICommand>();

        public void Dispatch(ICommand cmd)
        {
            CommandsToDispatch.Add(cmd);
        }

        private readonly IDictionary<Type,Event> messagesToEventsMap = new Dictionary<Type, Event>(); 
        protected override void Event<T>(Expression<Func<Event<T>>> propertyExpression)
        {
            messagesToEventsMap[typeof(T)] = propertyExpression.Compile().Invoke();
            base.Event(propertyExpression);
        }

        protected override void State(Expression<Func<State>> propertyExpression)
        {
            var state = propertyExpression.Compile().Invoke();
            WhenEnter(state, x => x.Then(ctx => ctx.Instance.CurrentState = state));
            base.State(propertyExpression);
        }

        public void RaiseByExternalEvent<TExternalEvent>(TSagaState progress, TExternalEvent externalEvent) where TExternalEvent : class
        {
            this.RaiseEvent(progress, GetMachineEvent(externalEvent), externalEvent);
        }

        private Event<TExternalEvent> GetMachineEvent<TExternalEvent>(TExternalEvent @event)
        {
            Event ev = null;
            if (!messagesToEventsMap.TryGetValue(typeof(TExternalEvent), out ev))
                throw new UnbindedMessageRecievedException(@event);
            return (Event<TExternalEvent>)ev;
        }
    }
}