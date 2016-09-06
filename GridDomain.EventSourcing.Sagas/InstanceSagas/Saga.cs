using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Automatonymous;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.Logging;

namespace GridDomain.EventSourcing.Sagas.InstanceSagas
{
    public class Saga<TSagaData> : AutomatonymousStateMachine<TSagaData> where TSagaData : class, ISagaState
    {
        private readonly List<Command> _commandsToDispatch = new List<Command>();
        public IReadOnlyCollection<Command> CommandsToDispatch => _commandsToDispatch;

        public void ClearCommands()
        {
            _commandsToDispatch.Clear();
        }

        public void Dispatch(Command cmd)
        {
            _commandsToDispatch.Add(cmd);
        }

        public Saga()
        {
            InstanceState(d => d.CurrentStateName);
        }

        private readonly List<Type> _dispatchedCommands = new List<Type>(); 
        private readonly List<MessageBind> _acceptedMessageMap = new List<MessageBind>(); 
        private readonly IDictionary<Type,Event> _messagesToEventsMap = new Dictionary<Type, Event>();
        public IReadOnlyCollection<MessageBind> AcceptedMessageMap => _acceptedMessageMap;


        public IReadOnlyCollection<Type> DispatchedCommands => _dispatchedCommands;
        protected void Command<TCommand>()
        {
            _dispatchedCommands.Add(typeof(TCommand));
        } 

        protected override void Event<TEventData>(Expression<Func<Event<TEventData>>> propertyExpression)
        {
            Event(propertyExpression, nameof(DomainEvent.SagaId));
        }

        protected void Event<TEventData>(Expression<Func<Event<TEventData>>> propertyExpression, Expression<Func<TEventData,object>> fieldExpr)
        {
            Event(propertyExpression, MemberNameExtractor.GetName(fieldExpr));
        }

        protected void Event<TEventData>(Expression<Func<Event<TEventData>>> propertyExpression, string fieldName)
        {
            var machineEvent = propertyExpression.Compile().Invoke();
            _messagesToEventsMap[typeof(TEventData)] = machineEvent;
            _acceptedMessageMap.Add(new MessageBind(typeof(TEventData),fieldName));

            base.Event(propertyExpression);

            DuringAny(
                     When(machineEvent).Then(
                         ctx =>
                             OnEventReceived.Invoke(this,
                                 new EventReceivedData<TEventData, TSagaData>(ctx.Event, ctx.Data, ctx.Instance))));
        }

        public event EventHandler<StateChangedData<TSagaData>> OnStateEnter = delegate { };
        public event EventHandler<EventReceivedData<TSagaData>> OnEventReceived = delegate { };
        public event EventHandler<MessageReceivedData<TSagaData>> OnMessageReceived = delegate { };

        protected override void State(Expression<Func<State>> propertyExpression)
        {
            var state = propertyExpression.Compile().Invoke();
            WhenEnter(state, x => x.Then(ctx => 
                OnStateEnter.Invoke(this, new StateChangedData<TSagaData>(state, ctx.Instance))));

            base.State(propertyExpression);
        }

        public void RaiseByMessage<TMessage>(TSagaData progress, TMessage message) where TMessage : class
        {
            if (message == null)
                throw new NullMessageTransitException(progress);

            OnMessageReceived.Invoke(this,new MessageReceivedData<TSagaData>(message, progress));

            var machineEvent = GetMachineEvent(message);

            try
            {
                this.RaiseEvent(progress, machineEvent, message).Wait(1000);
            }
            catch(Exception ex)
            {
                throw new SagaTransitionException(message, progress, ex);
            }
        }

        private Event<TMessage> GetMachineEvent<TMessage>(TMessage message)
        {
            Event ev;
            if (!_messagesToEventsMap.TryGetValue(typeof(TMessage), out ev))
                throw new UnbindedMessageReceivedException(message, typeof(TMessage));
            return (Event<TMessage>)ev;
        }
    }

    public class SagaTransitionException : Exception
    {
        public object TransitionMessage { get; }
        public ISagaState SagaData { get; }

        public SagaTransitionException(object message, ISagaState progress, Exception inner)
            :base("Saga transition raised an error",inner)
        {
            SagaData = progress;
            TransitionMessage = message;
        }
    }

    public class NullMessageTransitException : Exception
    {
        public readonly object SagaData;

        public NullMessageTransitException(object sagaData):base("Saga was transitioned by null message")
        {
            SagaData = sagaData;
        }
    }
}