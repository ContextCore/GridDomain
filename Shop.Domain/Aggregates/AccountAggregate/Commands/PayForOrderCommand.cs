using System;
using NMoneys;

namespace Shop.Domain.Aggregates.AccountAggregate.Commands
{
    public class PayForOrderCommand : ChargeAccountCommand
    {
        public PayForOrderCommand(Guid accountId, Money amount, Guid billId)
            : base(accountId, amount)
        {
            BillId = billId;
        }

        public Guid BillId { get; }
    }
}