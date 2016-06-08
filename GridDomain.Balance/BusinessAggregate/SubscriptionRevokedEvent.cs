using System;

namespace BusinessNews.Domain.BusinessAggregate
{
    public class SubscriptionRevokedEvent : SubscriptionChangedEvent
    {
        public SubscriptionRevokedEvent(Guid businessId, Guid subscriptionId) : base(businessId, subscriptionId)
        {
        }
    }
}