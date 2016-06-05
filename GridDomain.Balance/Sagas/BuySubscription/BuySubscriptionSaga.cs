using System;
using BusinessNews.Domain.Domain.BillAggregate;
using CommonDomain.Core;
using GridDomain.Balance.Domain.AccountAggregate.Commands;
using GridDomain.Balance.Domain.BusinessAggregate;
using GridDomain.Balance.Domain.OfferAggregate;
using GridDomain.CQRS;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Sagas;
using Stateless;

namespace GridDomain.Balance.Domain
{
   

    public class BuySubscriptionSagaStateAggregate : SagaStateAggregate<BuySubscriptionSaga.State, BuySubscriptionSaga.Transitions>
    {
        public BuySubscriptionSagaStateAggregate(Guid id, BuySubscriptionSaga.State state) : base(id, state)
        {
        }

        public void RememberOrder(SubscriptionOrderedEvent e)
        {
            RaiseEvent(new AccountRememberedEvent(e.AccountId, e.BusinessId, e.SuibscriptionId));
        }

        private void Apply(AccountRememberedEvent e)
        {
            AccountId = e.AccountId;
            BusinessId = e.BusinessId;
            SubscriptionId = e.SuibscriptionId;
        }

        public Guid BusinessId { get; private set; }

        public Guid AccountId { get; private set; }

        internal class AccountRememberedEvent
        {
            public Guid AccountId { get; }
            public Guid BusinessId { get; }
            public Guid SuibscriptionId { get; }

            public AccountRememberedEvent(Guid accountId, Guid businessId, Guid suibscriptionId)
            {
                AccountId = accountId;
                BusinessId = businessId;
                SuibscriptionId = suibscriptionId;
            }
        }

        public void RememberSubscription(SubscriptionCreatedEvent e)
        {
            RaiseEvent(new SubscriptionRememberedEvent(e.SubscriptionId));
        }

        private void Apply(SubscriptionRememberedEvent e)
        {
            SubscriptionId = e.SubscriptionId;
        }

        public Guid SubscriptionId { get; private set; }

        public class SubscriptionRememberedEvent
        {
            public Guid SubscriptionId { get; }

            public SubscriptionRememberedEvent(Guid subscriptionId)
            {
                SubscriptionId = subscriptionId;
            }
        }
    }


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
            var createSubscriptionTransition = RegisterEvent<SubscriptionOrderedEvent>(Transitions.CreateSubscription);
            Machine.Configure(State.SubscriptionSet)
                   .OnEntryFrom(createSubscriptionTransition,
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
                           StateData.RememberSubscription(e);
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