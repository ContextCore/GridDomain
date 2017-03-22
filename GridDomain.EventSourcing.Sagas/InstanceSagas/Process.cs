using System;
using System.Collections.Generic;
using System.Linq;
using Automatonymous;
using Automatonymous.Events;
using GridDomain.CQRS;

namespace GridDomain.EventSourcing.Sagas.InstanceSagas
{
    public class Process<TSagaData> : AutomatonymousStateMachine<TSagaData> where TSagaData : class, ISagaState
    {
        private readonly IDictionary<Type, Event> _messagesToEventsMap = new Dictionary<Type, Event>();
        public Action<Command> DispatchCallback { get; set; }

        protected Process(Action<Command> dispatchCallback = null)
        {
            DispatchCallback = dispatchCallback ?? (c => {});
            InstanceState(d => d.CurrentStateName);
            foreach (var machineEvent in Events.Where(e =>
                                                      {
                                                          var type = e.GetType();
                                                          return type.IsGenericType
                                                                 && typeof(DataEvent<>).IsAssignableFrom(type.GetGenericTypeDefinition());
                                                      }))
            {
                var domainEventType = machineEvent.GetType().GetGenericArguments().First();
                _messagesToEventsMap[domainEventType] = machineEvent;
            }
        }


        protected void Dispatch(Command cmd)
        {
            DispatchCallback(cmd);
        }

        public Event<TMessage> GetMachineEvent<TMessage>(TMessage message)
        {
            Event ev;
            if (!_messagesToEventsMap.TryGetValue(typeof(TMessage), out ev))
                throw new UnbindedMessageReceivedException(message, typeof(TMessage));
            return (Event<TMessage>) ev;
        }
    }
}