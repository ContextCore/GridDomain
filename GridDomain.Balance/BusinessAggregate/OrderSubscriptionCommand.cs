using System;
using GridDomain.CQRS;

namespace BusinessNews.Domain.BusinessAggregate
{
    public class OrderSubscriptionCommand : Command
    {
        public Guid BusinessId { get; }
        public Guid OfferId { get; }
        public Guid SubscriptionId { get; }

        public OrderSubscriptionCommand(Guid businessId, Guid offerId, Guid subscriptionId)
        {
            BusinessId = businessId;
            OfferId = offerId;
            SubscriptionId = subscriptionId;
        }
    }
}