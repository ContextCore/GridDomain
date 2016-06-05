using System;
using GridDomain.CQRS;

namespace GridDomain.Balance.Domain.BusinessAggregate
{
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