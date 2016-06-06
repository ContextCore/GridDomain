using System;
using BusinessNews.Domain.OfferAggregate;
using GridDomain.EventSourcing;

namespace BusinessNews.Domain.SubscriptionAggregate
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