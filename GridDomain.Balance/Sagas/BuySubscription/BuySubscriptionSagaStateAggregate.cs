using System;
using BusinessNews.Domain.BusinessAggregate;
using BusinessNews.Domain.SubscriptionAggregate;
using GridDomain.EventSourcing.Sagas;

namespace BusinessNews.Domain.Sagas.BuySubscription
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
}