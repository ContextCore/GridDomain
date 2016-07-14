using Automatonymous;
using GridDomain.Tests.Sagas.SubscriptionRenewSaga.Commands;
using GridDomain.Tests.Sagas.SubscriptionRenewSaga.Events;

namespace GridDomain.Tests.Sagas.Simplified
{
    class SubscriptionRenewSagaSimplified : SagaStateMachine<SubscriptionRenewSagaInstance>
    {
      
        public SubscriptionRenewSagaSimplified()
        {
            Event(() => SubscriptionExpired);
            Event(() => SubscriptionPaid);
            Event(() => SubscriptionChanged);
            Event(() => NotEnoughFunds);

            State(() => SubscriptionSet);
            State(() => PayingForSubscription);
            State(() => ChangingSubscription);

            //WhenEnter(SubscriptionSet, x => x.Then(ctx => ctx.Instance.CurrentState = SubscriptionSet));
            //WhenEnter(PayingForSubscription, x => x.Then(ctx => ctx.Instance.CurrentState = PayingForSubscription));
            //WhenEnter(ChangingSubscription, x => x.Then(ctx => ctx.Instance.CurrentState = ChangingSubscription));

            During(SubscriptionSet,
                When(SubscriptionExpired).Then(context =>
                {
                    context.Instance.SubscriptionId = context.Data.SourceId;
                    Dispatch(new PayForSubscriptionCommand(context.Data));
                })
                    .TransitionTo(PayingForSubscription));

            During(PayingForSubscription, 
                When(NotEnoughFunds)
                    .Then(context => Dispatch(new ChangeSubscriptionCommand(context.Data)))
                    .TransitionTo(ChangingSubscription),
                When(SubscriptionPaid)
                    .TransitionTo(SubscriptionSet));

            During(ChangingSubscription,
                When(SubscriptionChanged).TransitionTo(SubscriptionSet));
        }

        public Event<SubscriptionExpiredEvent> SubscriptionExpired { get; private set; } //Trigger.SubscriptionExpired;
        public Event<SubscriptionPaidEvent>    SubscriptionPaid    { get; private set; } //Trigger.SubscriptionPaid;
        public Event<SubscriptionChangedEvent> SubscriptionChanged { get; private set; } //Trigger.SubscriptionChanged;
        public Event<NotEnoughFundsFailure>    NotEnoughFunds      { get; private set; }//Trigger.RevokeSubscription

        public State SubscriptionSet       { get; private set; }       //State.SubscriptionSet
        public State PayingForSubscription { get; private set; } //State.OfferPaying
        public State ChangingSubscription  { get; private set; }  //State.SubscriptionChanging
    }
}