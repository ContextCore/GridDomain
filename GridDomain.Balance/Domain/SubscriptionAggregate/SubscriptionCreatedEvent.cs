using System;
using GridDomain.EventSourcing;
using NMoneys;

namespace GridDomain.Balance.Domain.OfferAggregate
{
    internal class SubscriptionCreatedEvent : DomainEvent
    {
        public Offer Offer { get; }

        public SubscriptionCreatedEvent(Guid sourceId, Offer offer, DateTime? createdTime = null)
            : base(sourceId, createdTime)
        {
            Offer = offer;
        }
    }
}