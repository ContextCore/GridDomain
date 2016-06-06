using System;
using GridDomain.CQRS;

namespace BusinessNews.Domain.BusinessAggregate
{
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