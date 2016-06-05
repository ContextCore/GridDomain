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
    public class CompleteBusinessSubscriptionOrderCommand : Command
    {
        public Guid BusinessId { get; }
        public Guid SubscriptionId { get; }

        public CompleteBusinessSubscriptionOrderCommand(Guid businessId, Guid subscriptionId)
        {
            BusinessId = businessId;
            SubscriptionId = subscriptionId;
        }
    }
}