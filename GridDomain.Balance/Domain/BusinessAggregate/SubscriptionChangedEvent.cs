using System;
using GridDomain.EventSourcing;

namespace GridDomain.Balance.Domain.BusinessAggregate
{
    public class SubscriptionChangedEvent : DomainEvent
    {
        protected SubscriptionChangedEvent(Guid businessId, Guid subscriptionId) : base(businessId)
        {
            SubscriptionId = subscriptionId;
        }

        public Guid BusinessId => SourceId;
        public Guid SubscriptionId { get; }
    }
}