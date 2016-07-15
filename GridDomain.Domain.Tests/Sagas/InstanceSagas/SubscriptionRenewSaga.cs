using Automatonymous;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Tests.Sagas.StateSagas.SampleSaga.Commands;
using GridDomain.Tests.Sagas.StateSagas.SampleSaga.Events;

namespace GridDomain.Tests.Sagas.InstanceSagas
{

    class SubscriptionRenewSaga : Saga<SubscriptionRenewSagaData>
    {
        
        public SubscriptionRenewSaga()
            :base()
        { 
            Event(() => SubscriptionExpired);
            Event(() => SubscriptionPaid);
            Event(() => SubscriptionChanged);
            Event(() => NotEnoughFunds);

            State(() => SubscriptionSet);
            State(() => PayingForSubscription);
            State(() => ChangingSubscription);

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

        public State SubscriptionSet       { get; private set; } //State.SubscriptionSet
        public State PayingForSubscription { get; private set; } //State.OfferPaying
        public State ChangingSubscription  { get; private set; } //State.SubscriptionChanging
    }
}