using System;
using System.Collections.Generic;
using BusinessNews.Domain.BillAggregate;
using BusinessNews.Domain.OfferAggregate;
using CommonDomain.Core;

namespace BusinessNews.Domain.SubscriptionAggregate
{
    public class Subscription : AggregateBase
    {
        private readonly List<Charge> _charges = new List<Charge>();

        private Subscription(Guid id)
        {
            Id = id;
        }

        public Subscription(Guid id, Offer offer) : this(id)
        {
            RaiseEvent(new SubscriptionCreatedEvent(id, offer));
        }

        public Offer Offer { get; private set; }
        public IReadOnlyCollection<Charge> Charges => _charges;

        public void Charge(Guid chargeId)
        {
            RaiseEvent(new SubscriptionChargedEvent(Id, chargeId, Offer.Price));
        }

        private void Apply(SubscriptionCreatedEvent e)
        {
            Id = e.SourceId;
            Offer = e.Offer;
        }

        private void Apply(SubscriptionChargedEvent e)
        {
            _charges.Add(new Charge(e.ChargeId, e.Price));
        }
    }
}