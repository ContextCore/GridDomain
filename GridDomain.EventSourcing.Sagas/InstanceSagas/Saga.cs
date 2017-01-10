using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Automatonymous;
using Automatonymous.Events;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.Logging;

namespace GridDomain.EventSourcing.Sagas.InstanceSagas
{
    public class Saga<TSagaData>: AutomatonymousStateMachine<TSagaData> where TSagaData : class, ISagaState
    {
        private readonly List<Command> _commandsToDispatch = new List<Command>();
        public IReadOnlyCollection<Command> CommandsToDispatch => _commandsToDispatch;

        public void ClearCommands()
        {
            _commandsToDispatch.Clear();
        }

        protected void Dispatch(Command cmd)
        {
            _commandsToDispatch.Add(cmd);
        }

        protected Saga()
        {
            InstanceState(d => d.CurrentStateName);
            foreach (var machineEvent in Events.Where(e =>
            {
                var type = e.GetType();
                return type.IsGenericType && typeof(DataEvent<>).IsAssignableFrom(type.GetGenericTypeDefinition());
            }))
            {
                MapDomainEvent((dynamic) machineEvent);
            }

            foreach (var state in States)
            {
                WhenEnter(state, x => x.Then(ctx =>  OnStateEnter.Invoke(this, new StateChangedData<TSagaData>(state, ctx.Instance))));
            }
        }

        private readonly IDictionary<Type,Event> _messagesToEventsMap = new Dictionary<Type, Event>();

        private void MapDomainEvent<TDomainEvent>(Event<TDomainEvent> fromMachineEvent)
        {
            _messagesToEventsMap[typeof(TDomainEvent)] = fromMachineEvent;

            DuringAny(
                     When(fromMachineEvent).Then(
                         ctx =>
                             OnEventReceived.Invoke(this,
                                 new EventReceivedData<TSagaData>(ctx.Event, ctx.Data, ctx.Instance))));
        }

        public event EventHandler<StateChangedData<TSagaData>> OnStateEnter = delegate { };
        public event EventHandler<EventReceivedData<TSagaData>> OnEventReceived = delegate { };
        public event EventHandler<MessageReceivedData<TSagaData>> OnMessageReceived = delegate { };

        public async Task RaiseByMessage<TMessage>(TSagaData progress, TMessage message) where TMessage : class
        {
            if (message == null)
                throw new NullMessageTransitException(progress);

            OnMessageReceived.Invoke(this,new MessageReceivedData<TSagaData>(message, progress));

            var machineEvent = GetMachineEvent(message);

            try
            {
                await this.RaiseEvent(progress, machineEvent, message);
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
}