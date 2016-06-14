using System;
using BusinessNews.Domain.BusinessAggregate;
using BusinessNews.Domain.SubscriptionAggregate;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Sagas;

namespace BusinessNews.Domain.Sagas.BuySubscription
{
    public class BuySubscriptionSagaStateAggregate :
        SagaStateAggregate<BuySubscriptionSaga.State, BuySubscriptionSaga.Transitions>
    {
        public BuySubscriptionSagaStateAggregate(Guid id, BuySubscriptionSaga.State state = BuySubscriptionSaga.State.SubscriptionSet) :base(id,state)
        {
        }

        private BuySubscriptionSagaStateAggregate(Guid id) : base(id)
        {
        }

        public Guid BusinessId { get; private set; }

        public Guid AccountId { get; private set; }

        public Guid SubscriptionId { get; private set; }

        public void RememberOrder(SubscriptionOrderedEvent e)
        {
            RaiseEvent(new AccountRememberedEvent(e.AccountId, e.BusinessId, e.SuibscriptionId, Id));
        }

        private void Apply(AccountRememberedEvent e)
        {
            AccountId = e.AccountId;
            BusinessId = e.BusinessId;
            SubscriptionId = e.SuibscriptionId;
        }

        public void RememberSubscription(SubscriptionCreatedEvent e)
        {
            RaiseEvent(new SubscriptionRememberedEvent(e.SubscriptionId,Id));
        }

        private void Apply(SubscriptionRememberedEvent e)
        {
            SubscriptionId = e.SubscriptionId;
        }
    }

 

    internal class AccountRememberedEvent : SagaStateEvent
    {
        public AccountRememberedEvent(Guid accountId, Guid businessId, Guid suibscriptionId, Guid sagaId) : base(sagaId)
        {
            AccountId = accountId;
            BusinessId = businessId;
            SuibscriptionId = suibscriptionId;
        }

        public Guid AccountId { get; }
        public Guid BusinessId { get; }
        public Guid SuibscriptionId { get; }
    }

    public class SubscriptionRememberedEvent: SagaStateEvent
    {
        public SubscriptionRememberedEvent(Guid subscriptionId, Guid sagaId) : base(sagaId)
        {
            SubscriptionId = subscriptionId;
        }

        public Guid SubscriptionId { get; }
    }
}