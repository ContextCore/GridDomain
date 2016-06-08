using System;
using System.Collections.Generic;
using System.Linq;
using CommonDomain;
using GridDomain.CQRS;
using Stateless;

namespace GridDomain.EventSourcing.Sagas
{

    public static class SagaInfo<TSaga>
    {
        public static IReadOnlyCollection<Type> KnownMessages()
        {
            var allInterfaces = typeof(TSaga).GetInterfaces();

            var handlerInterfaces =
                allInterfaces.Where(i => i.IsGenericType &&
                                         i.GetGenericTypeDefinition() == typeof(IHandler<>))
                             .ToArray();

            return handlerInterfaces.SelectMany(s => s.GetGenericArguments()).ToArray();
        }
    }

    public class SagaDescriptor
    {
        
    }
public class StateSaga<TSagaStates, TSagaTriggers, TStateData, TStartMessage> : 
                                                                 IDomainSaga,
                                                                 ISagaDescriptor
                                                                 where TSagaTriggers : struct
                                                                 where TSagaStates : struct
                                                                 where TStateData : SagaStateAggregate<TSagaStates, TSagaTriggers>
    {
        private readonly IDictionary<Type, StateMachine<TSagaStates, TSagaTriggers>.TriggerWithParameters>
            _eventsToTriggersMapping
                = new Dictionary<Type, StateMachine<TSagaStates, TSagaTriggers>.TriggerWithParameters>();

        public IReadOnlyCollection<Type> AcceptMessages => _eventsToTriggersMapping.Keys.ToArray();
        public Type StartMessage { get; } = typeof (TStartMessage);
        public Type StateType { get; } = typeof (TStateData);
        public Type SagaType => this.GetType();

        public readonly StateMachine<TSagaStates, TSagaTriggers> Machine;

        protected readonly TStateData StateData;

        protected StateSaga(TStateData stateData)
        {
            StateData = stateData;

            //to include start message into list of accept messages
            _eventsToTriggersMapping[typeof (TStartMessage)] = null; 
            Machine = new StateMachine<TSagaStates, TSagaTriggers>(StateData.MachineState);
            Machine.OnTransitioned(t => StateData.StateChanged(t.Trigger, t.Destination));
        }

        public TSagaStates DomainState => StateData.MachineState;
        public List<object> MessagesToDispatch => new List<object>();

        IAggregate IDomainSaga.StateAggregate => StateData;

        public void Handle(object msg)
        {
            Transit(msg);
        }

        protected void Dispatch(object message)
        {
            MessagesToDispatch.Add(message);
        }

        protected StateMachine<TSagaStates, TSagaTriggers>.TriggerWithParameters<TEvent> RegisterEvent<TEvent>(
                                                                                                    TSagaTriggers trigger)
        {
            var triggerWithParameters = Machine.SetTriggerParameters<TEvent>(trigger);
            _eventsToTriggersMapping[typeof(TEvent)] = triggerWithParameters;
            return triggerWithParameters;
        }

        //TODO: make method non-generic
        protected internal void Transit<T>(T message)
        {
            StateMachine<TSagaStates, TSagaTriggers>.TriggerWithParameters triggerWithParameters;
            if (!_eventsToTriggersMapping.TryGetValue(typeof (T), out triggerWithParameters))
                throw new UnbindedMessageRecievedException(message);

            if (Machine.CanFire(triggerWithParameters.Trigger))
                Machine.Fire((StateMachine<TSagaStates, TSagaTriggers>.TriggerWithParameters<T>) triggerWithParameters,
                    message);
        }
    }

    //TODO: add policy for command unexpected failure handling
    public class StateSaga<TState, TTrigger, TStartMessage> :
        StateSaga<TState, TTrigger, SagaStateAggregate<TState, TTrigger>, TStartMessage>
        where TTrigger : struct
        where TState : struct
    {
        public StateSaga(SagaStateAggregate<TState, TTrigger> stateData) : base(stateData)
        {
        }
    }
}