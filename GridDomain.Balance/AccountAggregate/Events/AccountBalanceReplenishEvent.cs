using System;
using NMoneys;

namespace BusinessNews.Domain.AccountAggregate.Events
{
    public class AccountBalanceReplenishEvent : AccountBalanceChangedEvent
    {
        public AccountBalanceReplenishEvent(Guid accountId, Money amount) : base(accountId, amount)
        {
        }
    }
}