using System;
using GridDomain.EventSourcing;
using NMoneys;

namespace GridDomain.Balance.Domain.OfferAggregate
{
    public class SubscriptionCreatedEvent : DomainEvent
    {
        public Offer Offer { get; }

        public Guid SubscriptionId => SourceId;
        public SubscriptionCreatedEvent(Guid subscriptionId, Offer offer)
            : base(subscriptionId)
        {
            Offer = offer;
        }
    }
}