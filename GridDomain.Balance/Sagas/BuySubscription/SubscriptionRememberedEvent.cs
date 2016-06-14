using System;
using GridDomain.EventSourcing.Sagas;

namespace BusinessNews.Domain.Sagas.BuySubscription
{
    public class SubscriptionRememberedEvent: SagaStateEvent
    {
        public SubscriptionRememberedEvent(Guid subscriptionId, Guid sagaId) : base(sagaId)
        {
            SubscriptionId = subscriptionId;
        }

        public Guid SubscriptionId { get; }
    }
}