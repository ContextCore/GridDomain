using System;
using System.Collections.Generic;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging.MessageRouting.Sagas;
using Stateless;

namespace GridDomain.Tests.Acceptance
{


    class StateFullSaga<TState,TTrigger,TStartMessage> : IStartBy<TStartMessage> where TTrigger : struct 
                                                                                 where TState : struct
    {
        private readonly SagaStateAggregate<TState, TTrigger> _state;

        private readonly IDictionary<Type, StateMachine<TState, TTrigger>.TriggerWithParameters> _eventsToTriggersMapping
            = new Dictionary<Type, StateMachine<TState, TTrigger>.TriggerWithParameters>();

        protected readonly StateMachine<TState, TTrigger> Machine;
        public List<object> EventsToDispatch  = new List<object>();
        
        public StateFullSaga(SagaStateAggregate<TState, TTrigger> state)
        {
            _state = state;
            Machine = new StateMachine<TState, TTrigger>(_state.State);
            Machine.OnTransitioned(t => _state.Transit(t.Trigger, t.Destination));
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
            EventsToDispatch.Add(message);
        }

        protected void Transit<T>(T message)
        {
            StateMachine<TState, TTrigger>.TriggerWithParameters trigger;
            if (!_eventsToTriggersMapping.TryGetValue(typeof(T), out trigger))
                throw new UnbindedMessageRecievedException(message);

            Machine.Fire((StateMachine<TState, TTrigger>.TriggerWithParameters<T>)trigger, message);
        }
    }

    class SubscriptionRenewSaga : StateFullSaga<SubscriptionRenewSaga.State, SubscriptionRenewSaga.Trigger, SubscriptionExpiredEvent>,
                                  IHandler<SubscriptionChangedEvent>,
                                  IHandler<NotEnoughFondsFailure>,
                                  IHandler<SubscriptionPaidEvent>
    {
        internal enum Trigger
        {
            PayForSubscription,
            RemainSubscription,
            ChangeSubscription,
            RevokeSubscription
        }

        internal enum State
        {
            SubscriptionSet,
            OfferPaying,
            SubscriptionChanging
        }


        public SubscriptionRenewSaga(SagaStateAggregate<State, Trigger> state):base(state)
        {
            var parForSubscriptionTrigger = RegisterEvent<SubscriptionExpiredEvent>(Trigger.PayForSubscription);
            var remainSubscriptionTrigger = RegisterEvent<SubscriptionPaidEvent>(Trigger.RemainSubscription);
            var changeSubscriptionTrigger = RegisterEvent<SubscriptionChangedEvent>(Trigger.ChangeSubscription);
            var revokeSubscriptionTrigger = RegisterEvent<NotEnoughFondsFailure>(Trigger.RevokeSubscription);

            Machine.Configure(State.SubscriptionSet)
                   .Permit(Trigger.PayForSubscription, State.OfferPaying);

            Machine.Configure(State.OfferPaying)
                   .OnEntryFrom(parForSubscriptionTrigger, e => Dispatch(new PayForSubscriptionCommand(e)))
                   .Permit(Trigger.RemainSubscription, State.SubscriptionSet)
                   .Permit(Trigger.RevokeSubscription, State.SubscriptionChanging);

            Machine.Configure(State.SubscriptionChanging)
                   .OnEntryFrom(revokeSubscriptionTrigger, e => Dispatch(new ChangeSubscriptionCommnad(e)))
                   .Permit(Trigger.ChangeSubscription, State.SubscriptionSet);
        }

        public void Handle(SubscriptionChangedEvent e)
        {
            Transit(e);
        }

        public void Handle(NotEnoughFondsFailure e)
        {
            Transit(e);
        }

        public void Handle(SubscriptionPaidEvent e)
        {
            Transit(e);
        }
    }
}