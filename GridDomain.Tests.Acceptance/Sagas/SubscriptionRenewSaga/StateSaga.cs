using System;
using System.Collections.Generic;
using GridDomain.CQRS.Messaging.MessageRouting.Sagas;
using Stateless;

namespace GridDomain.Tests.Acceptance
{
    class StateSaga<TState,TTrigger,TStartMessage> : IStartBy<TStartMessage> where TTrigger : struct 
                                                  where TState : struct
    {
        private readonly SagaStateAggregate<TState, TTrigger> _stateAggregate;

        public TState State => _stateAggregate.MachineState;

        private readonly IDictionary<Type, StateMachine<TState, TTrigger>.TriggerWithParameters> _eventsToTriggersMapping
            = new Dictionary<Type, StateMachine<TState, TTrigger>.TriggerWithParameters>();

        protected readonly StateMachine<TState, TTrigger> Machine;
        public List<object> MessagesToDispatch  = new List<object>();
        
        public StateSaga(SagaStateAggregate<TState, TTrigger> stateAggregate)
        {
            _stateAggregate = stateAggregate;
            Machine = new StateMachine<TState, TTrigger>(_stateAggregate.MachineState);
            Machine.OnTransitioned(t => _stateAggregate.Transit(t.Trigger, t.Destination));
        }

        protected StateMachine<TState, TTrigger>.TriggerWithParameters<TEvent> RegisterEvent<TEvent>(TTrigger trigger)
        {
            var triggerWithParameters = Machine.SetTriggerParameters<TEvent>(trigger);
            _eventsToTriggersMapping[typeof(TEvent)] = triggerWithParameters;
            return triggerWithParameters;
        }

        public void Handle(TStartMessage e)
        {
            Transit(e);
        }

        protected void Dispatch(object message)
        {
            MessagesToDispatch.Add(message);
        }

        internal void Transit<T>(T message)
        {
            StateMachine<TState, TTrigger>.TriggerWithParameters triggerWithParameters;
            if (!_eventsToTriggersMapping.TryGetValue(typeof(T), out triggerWithParameters))
                throw new UnbindedMessageRecievedException(message);

            if(Machine.CanFire(triggerWithParameters.Trigger))
               Machine.Fire((StateMachine<TState, TTrigger>.TriggerWithParameters<T>)triggerWithParameters, message);
        }
    }
}