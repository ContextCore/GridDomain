using System;
using NMoneys;

namespace GridDomain.Balance.Domain.AccountAggregate.Commands
{
    public class PayForBillCommand : ChangeAccountCommand
    {
        public Guid BillId { get; private set; }

        public PayForBillCommand(Guid balanceId, Money amount, Guid billId)
            : base(balanceId, amount)
        {
            BillId = billId;
        }
    }



}