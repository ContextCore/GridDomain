using System;
using GridDomain.CQRS;
using GridDomain.EventSourcing.Sagas;
using GridDomain.Tests.Sagas.SubscriptionRenewSaga.Commands;
using GridDomain.Tests.Sagas.SubscriptionRenewSaga.Events;

namespace GridDomain.Tests.Sagas.SubscriptionRenewSaga
{
    public class SubscriptionRenewSaga :
        StateSaga<SubscriptionRenewSaga.States, SubscriptionRenewSaga.Triggers, SubscriptionRenewSagaState, SubscriptionExpiredEvent>,
        IHandler<SubscriptionChangedEvent>,
        IHandler<NotEnoughFundsFailure>,
        IHandler<SubscriptionPaidEvent>
    {

        public static ISagaDescriptor Descriptor = new SubscriptionRenewSaga(new SubscriptionRenewSagaState(Guid.Empty,States.SubscriptionSet));
        public SubscriptionRenewSaga(SubscriptionRenewSagaState state) : base(state)
        {
            var parForSubscriptionTrigger = RegisterEvent<SubscriptionExpiredEvent>(Triggers.PayForSubscription);
            var remainSubscriptionTrigger = RegisterEvent<SubscriptionPaidEvent>(Triggers.RemainSubscription);
            var changeSubscriptionTrigger = RegisterEvent<SubscriptionChangedEvent>(Triggers.ChangeSubscription);
            var revokeSubscriptionTrigger = RegisterEvent<NotEnoughFundsFailure>(Triggers.RevokeSubscription);

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

        public void Handle(SubscriptionExpiredEvent e)
        {
            TransitState(e);
        }

        public void Handle(NotEnoughFundsFailure msg)
        {
            TransitState(msg);
        }

        public void Handle(SubscriptionChangedEvent msg)
        {
            TransitState(msg);
        }

        public void Handle(SubscriptionPaidEvent msg)
        {
            TransitState(msg);
        }

        public enum Triggers
        {
            PayForSubscription,
            RemainSubscription,
            ChangeSubscription,
            RevokeSubscription
        }

        public enum States
        {
            SubscriptionSet,
            OfferPaying,
            SubscriptionChanging
        }
    }
}