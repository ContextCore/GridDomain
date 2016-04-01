using System;
using NMoneys;

namespace GridDomain.Balance.Domain
{
    public class BalanceReplenishEvent: BalanceChangedEvent
    {
        public BalanceReplenishEvent(Guid balanceId, Money amount) : base(balanceId, amount)
        {
        }
    }
}