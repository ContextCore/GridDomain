using System;
using GridDomain.EventSourcing;
using NMoneys;

namespace BusinessNews.Domain.Domain.BillAggregate
{
    internal class BillCreatedEvent: DomainEvent
    {
        public Guid BillId => SourceId;
        public Money Amount { get; }

        public BillCreatedEvent(Guid id, Money amount):base(id)
        {
            Amount = amount;
        }
    }
}