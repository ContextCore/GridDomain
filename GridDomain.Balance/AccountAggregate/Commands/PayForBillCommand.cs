using System;
using NMoneys;

namespace BusinessNews.Domain.AccountAggregate.Commands
{
    public class PayForBillCommand : ChargeAccountCommand
    {
        public PayForBillCommand(Guid accountId, Money amount, Guid billId)
            : base(accountId, amount)
        {
            BillId = billId;
        }

        public Guid BillId { get; }
    }
}