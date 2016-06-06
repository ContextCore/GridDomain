using System;
using GridDomain.EventSourcing;
using NMoneys;

namespace BusinessNews.Domain.BillAggregate
{
    public class BillCreatedEvent: DomainEvent
    {
        public Charge[] Charges { get; }
        public Guid BillId => SourceId;
        public Money Amount { get; }

        public BillCreatedEvent(Guid id, Charge[] charges, Money amount):base(id)
        {
            this.Charges = charges;
            Amount = amount;
        }
    }
}