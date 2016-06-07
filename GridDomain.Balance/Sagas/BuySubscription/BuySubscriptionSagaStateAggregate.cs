using System;
using BusinessNews.Domain.BusinessAggregate;
using BusinessNews.Domain.SubscriptionAggregate;
using GridDomain.EventSourcing.Sagas;

namespace BusinessNews.Domain.Sagas.BuySubscription
{
    public class BuySubscriptionSagaStateAggregate :
        SagaStateAggregate<BuySubscriptionSaga.State, BuySubscriptionSaga.Transitions>
    {
        public BuySubscriptionSagaStateAggregate(Guid id, BuySubscriptionSaga.State state) : base(id, state)
        {
        }

        public Guid BusinessId { get; private set; }

        public Guid AccountId { get; private set; }

        public Guid SubscriptionId { get; private set; }

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

        public void RememberSubscription(SubscriptionCreatedEvent e)
        {
            RaiseEvent(new SubscriptionRememberedEvent(e.SubscriptionId));
        }

        private void Apply(SubscriptionRememberedEvent e)
        {
            SubscriptionId = e.SubscriptionId;
        }

        internal class AccountRememberedEvent
        {
            public AccountRememberedEvent(Guid accountId, Guid businessId, Guid suibscriptionId)
            {
                AccountId = accountId;
                BusinessId = businessId;
                SuibscriptionId = suibscriptionId;
            }

            public Guid AccountId { get; }
            public Guid BusinessId { get; }
            public Guid SuibscriptionId { get; }
        }

        public class SubscriptionRememberedEvent
        {
            public SubscriptionRememberedEvent(Guid subscriptionId)
            {
                SubscriptionId = subscriptionId;
            }

            public Guid SubscriptionId { get; }
        }
    }
}