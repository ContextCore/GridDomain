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
                var domainEventType = machineEvent.GetType().GetGenericArguments().First();
                _messagesToEventsMap[domainEventType] = machineEvent;
            }
        }
        
        private readonly IDictionary<Type,Event> _messagesToEventsMap = new Dictionary<Type, Event>();

        public Event<TMessage> GetMachineEvent<TMessage>(TMessage message)
        {
            Event ev;
            if (!_messagesToEventsMap.TryGetValue(typeof(TMessage), out ev))
                throw new UnbindedMessageReceivedException(message, typeof(TMessage));
            return (Event<TMessage>)ev;
        }
    }
}