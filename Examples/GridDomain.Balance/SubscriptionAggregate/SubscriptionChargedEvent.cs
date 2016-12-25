using System;
using GridDomain.EventSourcing;
using NMoneys;

namespace BusinessNews.Domain.SubscriptionAggregate
{
    public class SubscriptionChargedEvent : DomainEvent
    {
        public SubscriptionChargedEvent(Guid subscriptionId, Guid chargeId, Money price) : base(subscriptionId)
        {
            ChargeId = chargeId;
            Price = price;
        }

        public Guid ChargeId { get; }
        public Money Price { get; }
    }
}