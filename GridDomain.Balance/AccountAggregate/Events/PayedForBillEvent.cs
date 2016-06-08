using System;
using NMoneys;

namespace BusinessNews.Domain.AccountAggregate.Events
{
    public class PayedForBillEvent : AccountBalanceChangedEvent
    {
        public PayedForBillEvent(Guid balanceId, Money amount, Guid billId) : base(balanceId, amount)
        {
            BillId = billId;
        }

        public Guid BillId { get; }
    }
}