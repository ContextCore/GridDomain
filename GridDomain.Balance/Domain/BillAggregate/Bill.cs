using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonDomain.Core;
using NMoneys;

namespace BusinessNews.Domain.Domain.BillAggregate
{
    class Bill : AggregateBase
    {
        public Money Amount { get; private set; }
        public bool IsPaid { get; private set; }

        private Bill(Guid id)
        {
            Id = id;
        }

        public Bill(Guid id, Money amount)
        {
            RaiseEvent(new BillCreatedEvent(id, amount));
        }

        private void Apply(BillCreatedEvent e)
        {
            Id = e.BillId;
            Amount = e.Amount;
            IsPaid = false;
        }

        private void Apply(BillPayedEvent e)
        {
            IsPaid = true;
        }
    }
}
