using System;
using BusinessNews.Domain.AccountAggregate.Commands;
using BusinessNews.Domain.AccountAggregate.Events;
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
                                                 SubscriptionOrderedEvent>
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

        //TODO: extract subscriptor in separate class
        public static ISagaDescriptor Descriptor => 
            new BuySubscriptionSaga(new BuySubscriptionSagaStateAggregate(Guid.Empty, State.BillCreating));


        public BuySubscriptionSaga(BuySubscriptionSagaStateAggregate state) : base(state)
        {
            var subscriptionOrderCreatedTransition = RegisterEvent<SubscriptionOrderedEvent>(Transitions.CreateSubscription);
            var subscriptionOrderCompletedTransition = RegisterEvent<SubscriptionOrderCompletedEvent>(Transitions.SubscriptionSet);


            Machine.Configure(State.SubscriptionSet)
                .OnEntryFrom(subscriptionOrderCompletedTransition, e => 
                    base.State.Complete())
                   .Permit(Transitions.CreateSubscription, State.SubscriptionCreating);


            Machine.Configure(State.SubscriptionCreating)
                .OnEntryFrom(subscriptionOrderCreatedTransition,
                    e =>
                    {
                        base.State.RememberOrder(e);
                        Dispatch(new CreateSubscriptionCommand(e.SuibscriptionId, e.OfferId));
                    })
                .Permit(Transitions.BillScubscription, State.ChargingSubscription);
            var chargeSubscriptionTransition = RegisterEvent<SubscriptionCreatedEvent>(Transitions.BillScubscription);

            Machine.Configure(State.ChargingSubscription)
                .OnEntryFrom(chargeSubscriptionTransition,
                    e =>
                    {
                        Dispatch(new ChargeSubscriptionCommand(e.SubscriptionId, Guid.NewGuid()));
                    })
                .Permit(Transitions.CreateBill, State.BillCreating);

            var createBillTransition = RegisterEvent<SubscriptionChargedEvent>(Transitions.CreateBill);

            Machine.Configure(State.BillCreating)
                .OnEntryFrom(createBillTransition,
                    e =>
                    {
                        Dispatch(new CreateBillCommand(new[] {new Charge(e.ChargeId, e.Price)}, Guid.NewGuid()));
                    })
                .Permit(Transitions.PayBill, State.BillPaying);
            var payBillTransition = RegisterEvent<BillCreatedEvent>(Transitions.PayBill);


            Machine.Configure(State.BillPaying)
                .OnEntryFrom(payBillTransition,
                    e => 
                    Dispatch(new PayForBillCommand(base.State.AccountId, e.Amount, e.BillId)))
                .Permit(Transitions.ChangeSubscription, State.SubscriptionSetting);
            var changeSubscriptionTransition = RegisterEvent<PayedForBillEvent>(Transitions.ChangeSubscription);


            Machine.Configure(State.SubscriptionSetting)
                .OnEntryFrom(changeSubscriptionTransition,
                    e =>
                        Dispatch(new CompleteBusinessSubscriptionOrderCommand(base.State.BusinessId,base.State.SubscriptionId)))
                .Permit(Transitions.SubscriptionSet, State.SubscriptionSet);


        }
    }
}