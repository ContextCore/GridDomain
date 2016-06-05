using System;
using GridDomain.CQRS;

namespace GridDomain.Balance.Domain.BusinessAggregate
{

    public class OrderSubscriptionCommand : Command
    {
        public Guid BusinessId { get; }
        public Guid SubscriptionId { get; }

        public OrderSubscriptionCommand(Guid businessId, Guid subscriptionId)
        {
            BusinessId = businessId;
            SubscriptionId = subscriptionId;
        }
    }
    public class SetBusinessSubscriptionCommand : Command
    {
        public Guid BusinessId { get; }
        public Guid SubscriptionId { get; }

        public SetBusinessSubscriptionCommand(Guid businessId, Guid subscriptionId)
        {
            BusinessId = businessId;
            SubscriptionId = subscriptionId;
        }
    }
}