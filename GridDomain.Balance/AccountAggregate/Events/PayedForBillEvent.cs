using System;
using NMoneys;

namespace BusinessNews.Domain.AccountAggregate.Events
{
    public class PayedForBillEvent : AccountBalanceChangedEvent
    {
        public Guid BillId { get; }

        public PayedForBillEvent(Guid balanceId, Money amount, Guid billId) : base(balanceId, amount)
        {
            BillId = billId;
        }
    }
}