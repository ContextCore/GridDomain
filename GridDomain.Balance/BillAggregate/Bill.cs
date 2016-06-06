using System;
using BusinessNews.Domain.SubscriptionAggregate;
using CommonDomain.Core;
using GridDomain.CQRS;
using NMoneys;

namespace BusinessNews.Domain.BillAggregate
{

    public struct Charge
    {
        public Charge(Guid id, Money amount)
        {
            Id = id;
            Amount = amount;
        }

        public Guid Id { get; }
        public Money Amount { get; }
    }
    public class CreateBillCommand :Command
    {
        public Guid BillId { get; }
        public Charge[] Charges { get; }

        public CreateBillCommand(Charge[] charges, Guid billId)
        {
            Charges = charges;
            BillId = billId;
        }
    }
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
            RaiseEvent(new BillCreatedEvent(id,amount));
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
