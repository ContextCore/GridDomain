using System;
using NMoneys;

namespace BusinessNews.Domain.AccountAggregate.Events
{
    public class AccountBalanceReplenishEvent : AccountBalanceChangedEvent
    {
        public AccountBalanceReplenishEvent(Guid balanceId, Money amount) : base(balanceId, amount)
        {
        }
    }
}