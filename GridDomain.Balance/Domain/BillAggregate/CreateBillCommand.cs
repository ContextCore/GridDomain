using System;
using GridDomain.CQRS;
using NMoneys;

namespace BusinessNews.Domain.Domain.BillAggregate
{
    class CreateBillCommand:Command
    {
        public CreateBillCommand(Guid id,Guid billId, Money amount):base(id)
        {
            BillId = billId;
            Amount = amount;
        }

        public Guid BillId { get; }
        public Money Amount { get; }
    }
}