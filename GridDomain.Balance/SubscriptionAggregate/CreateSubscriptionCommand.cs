using System;
using GridDomain.CQRS;

namespace BusinessNews.Domain.SubscriptionAggregate
{
    public class CreateSubscriptionCommand : Command
    {
        public CreateSubscriptionCommand(Guid subscriptionId, Guid offer)
        {
            Offer = offer;
            SubscriptionId = subscriptionId;
        }

        public Guid SubscriptionId { get; }
        public Guid Offer { get; }
    }
}