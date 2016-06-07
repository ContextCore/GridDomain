using System;
using GridDomain.CQRS;

namespace BusinessNews.Domain.BusinessAggregate
{
    public class CompleteBusinessSubscriptionOrderCommand : Command
    {
        public CompleteBusinessSubscriptionOrderCommand(Guid businessId, Guid subscriptionId)
        {
            BusinessId = businessId;
            SubscriptionId = subscriptionId;
        }

        public Guid BusinessId { get; }
        public Guid SubscriptionId { get; }
    }
}