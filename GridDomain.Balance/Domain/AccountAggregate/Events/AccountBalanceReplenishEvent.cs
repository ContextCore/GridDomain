using System;
using NMoneys;

namespace GridDomain.Balance.Domain.BalanceAggregate.Events
{
    public class AccountBalanceReplenishEvent : AccountBalanceChangedEvent
    {
        public AccountBalanceReplenishEvent(Guid balanceId, Money amount) : base(balanceId, amount)
        {
        }
    }
}