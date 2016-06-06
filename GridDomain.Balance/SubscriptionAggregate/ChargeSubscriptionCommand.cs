using System;
using GridDomain.CQRS;

namespace BusinessNews.Domain.SubscriptionAggregate
{
    public class ChargeSubscriptionCommand : Command
    {
        public Guid SubscriptionId { get; }
        public Guid ChargeId { get; }

        public ChargeSubscriptionCommand(Guid subscriptionId, Guid chargeId)
        {
            SubscriptionId = subscriptionId;
            ChargeId = chargeId;
        }
    }
}