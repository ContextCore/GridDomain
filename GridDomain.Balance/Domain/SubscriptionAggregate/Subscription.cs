using System;
using System.Collections.Generic;
using CommonDomain.Core;
using GridDomain.EventSourcing;
using NMoneys;

namespace GridDomain.Balance.Domain.OfferAggregate
{
    public class Subscription : AggregateBase
    {
        public Offer Offer { get; private set; }
        public IReadOnlyCollection<Guid> Bills => _bills;
        private readonly List<Guid> _bills = new List<Guid>();

        private Subscription(Guid id)
        {
            Id = id;
        }
    
        public Subscription(Guid id, Offer offer) : this(id)
        {
            RaiseEvent(new SubscriptionCreatedEvent(id,offer));
        }

        public void CreateBill(Guid billId)
        {
            RaiseEvent(new SubscriptionBillCreatedEvent(Id, billId, Offer.Price));
        }

        private void Apply(SubscriptionCreatedEvent e)
        {
            Id = e.SourceId;
            Offer = e.Offer;
        }
        private void Apply(SubscriptionBillCreatedEvent e)
        {
            _bills.Add(e.BillId);
        }
    }

    public class SubscriptionBillCreatedEvent:DomainEvent
    {
        public Guid BillId { get; }
        public Money Price { get; }

        public SubscriptionBillCreatedEvent(Guid id, Guid billId, Money price):base(id)
        {
            BillId = billId;
            Price = price;
        }
    }
}