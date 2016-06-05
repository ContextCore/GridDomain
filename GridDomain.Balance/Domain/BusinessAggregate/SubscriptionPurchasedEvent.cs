using System;

namespace GridDomain.Balance.Domain.BusinessAggregate
{
    public class SubscriptionPurchasedEvent : SubscriptionChangedEvent
    {
        public SubscriptionPurchasedEvent(Guid businessId, Guid subscriptionId) : base(businessId, subscriptionId)
        {
        }
    }
}