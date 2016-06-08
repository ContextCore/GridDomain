using System;

namespace BusinessNews.Domain.BusinessAggregate
{
    public class SubscriptionOrderCompletedEvent : SubscriptionChangedEvent
    {
        public SubscriptionOrderCompletedEvent(Guid businessId, Guid subscriptionId) : base(businessId, subscriptionId)
        {
        }
    }
}