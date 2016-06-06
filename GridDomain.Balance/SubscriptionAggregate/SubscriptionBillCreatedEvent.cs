using System;
using GridDomain.EventSourcing;
using NMoneys;

namespace BusinessNews.Domain.SubscriptionAggregate
{
    public class SubscriptionBillCreatedEvent:DomainEvent
    {
        public Guid BillId { get; }
        public Money Price { get; }

        public SubscriptionBillCreatedEvent(Guid subscriptionId, Guid billId, Money price):base(subscriptionId)
        {
            BillId = billId;
            Price = price;
        }
    }
}