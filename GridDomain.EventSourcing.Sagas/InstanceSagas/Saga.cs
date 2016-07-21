using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Automatonymous;
using GridDomain.CQRS;

namespace GridDomain.EventSourcing.Sagas.InstanceSagas
{
    public class Saga<TSagaData> : AutomatonymousStateMachine<TSagaData> where TSagaData : class, ISagaState<State>
    {
        private readonly Type _startMessageType;
        public readonly List<ICommand> CommandsToDispatch = new List<ICommand>();

        public void Dispatch(ICommand cmd)
        {
            CommandsToDispatch.Add(cmd);
        }

        public Saga(Type startMessageType)
        {
            _startMessageType = startMessageType;
        }

        public Type StartMessage => _startMessageType;
        private readonly List<Type> _dispatchedCommands = new List<Type>(); 
        private readonly IDictionary<Type,Event> _messagesToEventsMap = new Dictionary<Type, Event>();
        public IReadOnlyCollection<Type> DispatchedCommands => _dispatchedCommands;
        protected void Command<TCommand>()
        {
            _dispatchedCommands.Add(typeof(TCommand));
        } 

        protected override void Event<TEventData>(Expression<Func<Event<TEventData>>> propertyExpression)
        {
            var machineEvent = propertyExpression.Compile().Invoke();
            _messagesToEventsMap[typeof(TEventData)] = machineEvent;

            base.Event(propertyExpression);
            DuringAny(
                     When(machineEvent).Then(
                         ctx =>
                             OnEventReceived.Invoke(this,
                                 new EventReceivedData<TEventData, TSagaData>(ctx.Event, ctx.Data, ctx.Instance))));
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
                throw new UnbindedMessageReceivedException(@event, typeof(TExternalEvent));
            return (Event<TExternalEvent>)ev;
        }
    }
}