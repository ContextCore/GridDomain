using System;
using NMoneys;

namespace BusinessNews.Domain.AccountAggregate.Commands
{
    public class PayForBillCommand : ChangeAccountCommand
    {
        public Guid BillId { get; private set; }

        public PayForBillCommand(Guid accountId, Money amount, Guid billId)
            : base(accountId, amount)
        {
            BillId = billId;
        }
    }



}