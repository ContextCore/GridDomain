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
                                                 IHandler<SubscriptionBillCreatedEvent>,
                                                 IHandler<BillPayedEvent>,
                                                 IHandler<SubscriptionOrderCompletedEvent>
    {

        public enum State
        {
            SubscriptionSet,
            SubscriptionCreating,
            BillPaying,
            BillCreating,
            SubscriptionSetting
        }

        public enum Transitions
        {
            CreateBill,
            CreateSubscription,
            PayBill,
            ChangeSubscription,
            SetNewSubscription,
            SetFailed,
            SubscriptionSet
        }

        public BuySubscriptionSaga(BuySubscriptionSagaStateAggregate stateData):base(stateData)
        {
            var subscriptionOrderedTransition = RegisterEvent<SubscriptionOrderedEvent>(Transitions.CreateSubscription);
            Machine.Configure(State.SubscriptionSet)
                   //.OnE(subscriptionOrderedTransition, e =>
                   //{
                   //        Machine.Fire(subscriptionOrderedTransition,e);
                   //})
                   .Permit(Transitions.CreateSubscription, State.SubscriptionCreating);


            Machine.Configure(State.SubscriptionCreating)
                   .OnEntryFrom(subscriptionOrderedTransition,
                       e =>
                       {
                           StateData.RememberOrder(e);
                           Dispatch(new CreateSubscriptionCommand(e.SuibscriptionId, e.OfferId));
                       })
                   .Permit(Transitions.CreateBill, State.BillCreating);
            var createBillTransition = RegisterEvent<SubscriptionCreatedEvent>(Transitions.CreateBill);

            Machine.Configure(State.BillCreating)
                   .OnEntryFrom(createBillTransition,
                       e =>
                       {
                           //StateData.RememberSubscription(e);
                           Dispatch(new CreateSubscriptionBillCommand(e.SubscriptionId, Guid.NewGuid()));
                       })
                   .Permit(Transitions.PayBill, State.BillPaying);
            var payBillTransition = RegisterEvent<SubscriptionBillCreatedEvent>(Transitions.PayBill);


            Machine.Configure(State.BillPaying)
                   .OnEntryFrom(payBillTransition,
                                e => Dispatch(new PayForBillCommand(StateData.AccountId,e.Price,e.BillId)))
                   .Permit(Transitions.ChangeSubscription, State.SubscriptionSetting);
            var changeSubscriptionTransition = RegisterEvent<BillPayedEvent>(Transitions.ChangeSubscription);


            Machine.Configure(State.SubscriptionSetting)
                   .OnEntryFrom(changeSubscriptionTransition, 
                                e => Dispatch(new CompleteBusinessSubscriptionOrderCommand(StateData.BusinessId,StateData.SubscriptionId)))
                   .Permit(Transitions.SubscriptionSet, State.SubscriptionSet);
            var subscriptionSetTransition = RegisterEvent<SubscriptionOrderCompletedEvent>(Transitions.SubscriptionSet);

        }

        public void Handle(SubscriptionCreatedEvent e)
        {
            Transit(e);
        }

        public void Handle(SubscriptionBillCreatedEvent e)
        {
            Transit(e);
        }

        public void Handle(BillPayedEvent e)
        {
            Transit(e);
        }

        public void Handle(SubscriptionOrderCompletedEvent e)
        {
           Transit(e);
        }
    }

  
}