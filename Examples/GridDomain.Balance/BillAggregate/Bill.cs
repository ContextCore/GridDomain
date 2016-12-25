using System;
using System.Linq;
using CommonDomain.Core;
using NMoneys;

namespace BusinessNews.Domain.BillAggregate
{
    public class Bill : AggregateBase
    {
        private Bill(Guid id)
        {
            Id = id;
        }

        public Bill(Guid id, Charge[] charges)
        {
            var amount = charges.Aggregate(Money.Zero(), (m, c) => m += c.Amount);
            RaiseEvent(new BillCreatedEvent(id, charges, amount));
        }

        public Money Amount { get; private set; }
        public bool IsPaid { get; private set; }

        public void MarkPaid()
        {
            RaiseEvent(new BillMarkedAsPayedEvent(Id));
        }

        private void Apply(BillCreatedEvent e)
        {
            Id = e.BillId;
            Amount = e.Amount;
            IsPaid = false;
        }

        private void Apply(BillMarkedAsPayedEvent e)
        {
            IsPaid = true;
        }
    }
}