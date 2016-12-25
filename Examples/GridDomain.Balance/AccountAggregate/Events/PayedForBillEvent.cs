using System;
using NMoneys;

namespace BusinessNews.Domain.AccountAggregate.Events
{
    public class PayedForBillEvent : AccountBalanceChangedEvent
    {
        public PayedForBillEvent(Guid accountId, Money amount, Guid billId) : base(accountId, amount)
        {
            BillId = billId;
        }

        public Guid BillId { get; }
    }
}