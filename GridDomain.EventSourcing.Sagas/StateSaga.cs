using System;
using System.Collections.Generic;
using Stateless;

namespace GridDomain.EventSourcing.Sagas
{

    public class Saga
    {
        public readonly List<object> MessagesToDispatch = new List<object>();
        protected void Dispatch(object message)
        {
            MessagesToDispatch.Add(message);
        }

    }
    public class StateSaga<TSagaStates, TSagaTriggers, TStateData, TStartMessage> : Saga,
        IStartBy<TStartMessage> 
        where TSagaTriggers : struct
        where TSagaStates : struct
        where TStateData : SagaStateAggregate<TSagaStates,TSagaTriggers>
    {
        private readonly IDictionary<Type, StateMachine<TSagaStates, TSagaTriggers>.TriggerWithParameters>
         _eventsToTriggersMapping
             = new Dictionary<Type, StateMachine<TSagaStates, TSagaTriggers>.TriggerWithParameters>();

        protected readonly TStateData StateData;

        public readonly StateMachine<TSagaStates, TSagaTriggers> Machine;

        protected StateSaga(TStateData stateData)
        {
            StateData = stateData;
            Machine = new StateMachine<TSagaStates, TSagaTriggers>(StateData.MachineState);
            Machine.OnTransitioned(t => StateData.StateChanged(t.Trigger, t.Destination));
        }

        public TSagaStates DomainState => StateData.MachineState;

        public void Handle(TStartMessage e)
        {
            Transit(e);
        }

        protected StateMachine<TSagaStates, TSagaTriggers>.TriggerWithParameters<TEvent> RegisterEvent<TEvent>(TSagaTriggers trigger)
        {
            var triggerWithParameters = Machine.SetTriggerParameters<TEvent>(trigger);
            _eventsToTriggersMapping[typeof(TEvent)] = triggerWithParameters;
            return triggerWithParameters;
        }



        protected internal void Transit<T>(T message)
        {
            StateMachine<TSagaStates, TSagaTriggers>.TriggerWithParameters triggerWithParameters;
            if (!_eventsToTriggersMapping.TryGetValue(typeof(T), out triggerWithParameters))
                throw new UnbindedMessageRecievedException(message);

            if (Machine.CanFire(triggerWithParameters.Trigger))
                Machine.Fire((StateMachine<TSagaStates, TSagaTriggers>.TriggerWithParameters<T>)triggerWithParameters, message);
        }
    }

    //TODO: add policy for command unexpected failure handling
        public class StateSaga<TState, TTrigger, TStartMessage> : 
        StateSaga<TState,TTrigger,SagaStateAggregate<TState,TTrigger>,TStartMessage> 
        where TTrigger : struct
        where TState : struct
    {
            public StateSaga(SagaStateAggregate<TState, TTrigger> stateData) : base(stateData)
            {
            }
    }
}