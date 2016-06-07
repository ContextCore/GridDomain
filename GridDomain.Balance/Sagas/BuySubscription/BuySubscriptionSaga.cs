using System;
using BusinessNews.Domain.AccountAggregate.Commands;
using BusinessNews.Domain.BillAggregate;
using BusinessNews.Domain.BusinessAggregate;
using BusinessNews.Domain.SubscriptionAggregate;
using GridDomain.CQRS;
using GridDomain.EventSourcing.Sagas;

namespace BusinessNews.Domain.Sagas.BuySubscription
{
    public class BuySubscriptionSaga : StateSaga<BuySubscriptionSaga.State,
                                                 BuySubscriptionSaga.Transitions,
                                                 BuySubscriptionSagaStateAggregate,
                                                 SubscriptionOrderedEvent>,
        IHandler<SubscriptionCreatedEvent>,
        IHandler<SubscriptionChargedEvent>,
        IHandler<BillCreatedEvent>,
        IHandler<BillPayedEvent>,
        IHandler<SubscriptionOrderCompletedEvent>
    {
        public enum State
        {
            SubscriptionSet,
            SubscriptionCreating,
            BillPaying,
            BillCreating,
            SubscriptionSetting,
            ChargingSubscription
        }

        public enum Transitions
        {
            CreateBill,
            CreateSubscription,
            PayBill,
            ChangeSubscription,
            SetNewSubscription,
            SetFailed,
            SubscriptionSet,
            BillScubscription
        }

        public BuySubscriptionSaga(BuySubscriptionSagaStateAggregate stateData) : base(stateData)
        {
            var subscriptionOrderedTransition = RegisterEvent<SubscriptionOrderedEvent>(Transitions.CreateSubscription);
            Machine.Configure(State.SubscriptionSet)
                .Permit(Transitions.CreateSubscription, State.SubscriptionCreating);


            Machine.Configure(State.SubscriptionCreating)
                .OnEntryFrom(subscriptionOrderedTransition,
                    e =>
                    {
                        StateData.RememberOrder(e);
                        Dispatch(new CreateSubscriptionCommand(e.SuibscriptionId, e.OfferId));
                    })
                .Permit(Transitions.BillScubscription, State.ChargingSubscription);
            var chargeSubscriptionTransition = RegisterEvent<SubscriptionCreatedEvent>(Transitions.BillScubscription);

            Machine.Configure(State.ChargingSubscription)
                .OnEntryFrom(chargeSubscriptionTransition,
                    e => { Dispatch(new ChargeSubscriptionCommand(e.SubscriptionId, Guid.NewGuid())); })
                .Permit(Transitions.CreateBill, State.BillCreating);

            var createBillTransition = RegisterEvent<SubscriptionChargedEvent>(Transitions.CreateBill);

            Machine.Configure(State.BillCreating)
                .OnEntryFrom(createBillTransition,
                    e => { Dispatch(new CreateBillCommand(new[] {new Charge(e.ChargeId, e.Price)}, Guid.NewGuid())); })
                .Permit(Transitions.PayBill, State.BillPaying);
            var payBillTransition = RegisterEvent<BillCreatedEvent>(Transitions.PayBill);


            Machine.Configure(State.BillPaying)
                .OnEntryFrom(payBillTransition,
                    e => Dispatch(new PayForBillCommand(StateData.AccountId, e.Amount, e.BillId)))
                .Permit(Transitions.ChangeSubscription, State.SubscriptionSetting);
            var changeSubscriptionTransition = RegisterEvent<BillPayedEvent>(Transitions.ChangeSubscription);


            Machine.Configure(State.SubscriptionSetting)
                .OnEntryFrom(changeSubscriptionTransition,
                    e =>
                        Dispatch(new CompleteBusinessSubscriptionOrderCommand(StateData.BusinessId,
                            StateData.SubscriptionId)))
                .Permit(Transitions.SubscriptionSet, State.SubscriptionSet);
            var subscriptionSetTransition = RegisterEvent<SubscriptionOrderCompletedEvent>(Transitions.SubscriptionSet);
        }

        public void Handle(BillCreatedEvent msg)
        {
            Transit(msg);
        }

        public void Handle(BillPayedEvent msg)
        {
            Transit(msg);
        }

        public void Handle(SubscriptionChargedEvent msg)
        {
            Transit(msg);
        }

        public void Handle(SubscriptionCreatedEvent msg)
        {
            Transit(msg);
        }

        public void Handle(SubscriptionOrderCompletedEvent msg)
        {
            Transit(msg);
        }
    }
}