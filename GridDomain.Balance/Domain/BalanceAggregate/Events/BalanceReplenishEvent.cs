using System;
using NMoneys;

namespace GridDomain.Balance.Domain.BalanceAggregate.Events
{
    public class BalanceReplenishEvent : BalanceChangedEvent
    {
        public BalanceReplenishEvent(Guid balanceId, Money amount) : base(balanceId, amount)
        {
        }
    }
}