using System;
using NMoneys;

namespace GridDomain.Balance.Domain
{
    public class BalanceWithdrawalEvent : BalanceChangedEvent
    {
        public BalanceWithdrawalEvent(Guid balanceId, Money amount) : base(balanceId, amount)
        {
        }
    }
}