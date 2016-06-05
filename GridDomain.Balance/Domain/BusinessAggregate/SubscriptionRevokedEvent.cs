using System;

namespace GridDomain.Balance.Domain.BusinessAggregate
{
    public class SubscriptionRevokedEvent : SubscriptionChangedEvent
    {
        public SubscriptionRevokedEvent(Guid businessId, Guid subscriptionId) : base(businessId, subscriptionId)
        {
        }
    }
}