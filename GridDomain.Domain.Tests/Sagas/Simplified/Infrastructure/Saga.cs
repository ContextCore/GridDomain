using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Automatonymous;
using GridDomain.CQRS;
using GridDomain.EventSourcing.Sagas;

namespace GridDomain.Tests.Sagas.Simplified
{
    public class Saga<TSagaData> : AutomatonymousStateMachine<TSagaData> where TSagaData : class, ISagaState<State>
    {
        public readonly List<ICommand> CommandsToDispatch = new List<ICommand>();

        public void Dispatch(ICommand cmd)
        {
            CommandsToDispatch.Add(cmd);
        }

        private readonly IDictionary<Type,Event> _messagesToEventsMap = new Dictionary<Type, Event>();

        protected override void Event<TEventData>(Expression<Func<Event<TEventData>>> propertyExpression)
        {
            var machineEvent = propertyExpression.Compile().Invoke();
            _messagesToEventsMap[typeof(TEventData)] = machineEvent;

            When(machineEvent).Then(
                ctx =>
                    OnEventReceived.Invoke(this,
                        new EventReceivedData<TEventData, TSagaData>(ctx.Event, ctx.Data, ctx.Instance)));

            base.Event(propertyExpression);
        }
        public event EventHandler<StateChangedData<TSagaData>> OnStateEnter = delegate { };
        public event EventHandler<EventReceivedData<TSagaData>> OnEventReceived = delegate { };

        protected override void State(Expression<Func<State>> propertyExpression)
        {
            var state = propertyExpression.Compile().Invoke();
            WhenEnter(state, x => x.Then(ctx => 
                OnStateEnter.Invoke(this, new StateChangedData<TSagaData>(state, ctx.Instance))));

            base.State(propertyExpression);
        }

        public void RaiseByExternalEvent<TExternalEvent>(TSagaData progress, TExternalEvent externalEvent) where TExternalEvent : class
        {
            this.RaiseEvent(progress, GetMachineEvent(externalEvent), externalEvent);
        }

        private Event<TExternalEvent> GetMachineEvent<TExternalEvent>(TExternalEvent @event)
        {
            Event ev = null;
            if (!_messagesToEventsMap.TryGetValue(typeof(TExternalEvent), out ev))
                throw new UnbindedMessageRecievedException(@event);
            return (Event<TExternalEvent>)ev;
        }
    }
}