using System;

namespace GridDomain.Balance.Domain.BusinessAggregate
{
    public class SubscriptionOrderCompletedEvent : SubscriptionChangedEvent
    {
        public SubscriptionOrderCompletedEvent(Guid businessId, Guid subscriptionId) : base(businessId, subscriptionId)
        {
        }
    }
}