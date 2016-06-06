using GridDomain.CQRS;
using GridDomain.EventSourcing.Sagas;
using GridDomain.Tests.Sagas.SubscriptionRenewSaga.Commands;
using GridDomain.Tests.Sagas.SubscriptionRenewSaga.Events;

namespace GridDomain.Tests.Sagas.SubscriptionRenewSaga
{
    internal class SubscriptionRenewSaga :
        StateSaga<SubscriptionRenewSaga.States, SubscriptionRenewSaga.Triggers, SubscriptionExpiredEvent>,
        IHandler<SubscriptionChangedEvent>,
        IHandler<NotEnoughFondsFailure>,
        IHandler<SubscriptionPaidEvent>
    {
        public SubscriptionRenewSaga(SagaStateAggregate<States, Triggers> stateData) : base(stateData)
        {
            var parForSubscriptionTrigger = RegisterEvent<SubscriptionExpiredEvent>(Triggers.PayForSubscription);
            var remainSubscriptionTrigger = RegisterEvent<SubscriptionPaidEvent>(Triggers.RemainSubscription);
            var changeSubscriptionTrigger = RegisterEvent<SubscriptionChangedEvent>(Triggers.ChangeSubscription);
            var revokeSubscriptionTrigger = RegisterEvent<NotEnoughFondsFailure>(Triggers.RevokeSubscription);

            Machine.Configure(States.SubscriptionSet)
                .Permit(Triggers.PayForSubscription, States.OfferPaying);

            Machine.Configure(States.OfferPaying)
                .OnEntryFrom(parForSubscriptionTrigger, e => Dispatch(new PayForSubscriptionCommand(e)))
                .Permit(Triggers.RemainSubscription, States.SubscriptionSet)
                .Permit(Triggers.RevokeSubscription, States.SubscriptionChanging);

            Machine.Configure(States.SubscriptionChanging)
                .OnEntryFrom(revokeSubscriptionTrigger, e => Dispatch(new ChangeSubscriptionCommand(e)))
                .Permit(Triggers.ChangeSubscription, States.SubscriptionSet);
        }

        public void Handle(NotEnoughFondsFailure e)
        {
            Transit(e);
        }

        public void Handle(SubscriptionChangedEvent e)
        {
            Transit(e);
        }

        public void Handle(SubscriptionPaidEvent e)
        {
            Transit(e);
        }

        internal enum Triggers
        {
            PayForSubscription,
            RemainSubscription,
            ChangeSubscription,
            RevokeSubscription
        }

        internal enum States
        {
            SubscriptionSet,
            OfferPaying,
            SubscriptionChanging
        }
    }
}