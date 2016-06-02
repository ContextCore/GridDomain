using System;
using NMoneys;

namespace GridDomain.Balance.Domain.AccountAggregate.Events
{
    public class AccountWithdrawalEvent : AccountBalanceChangedEvent
    {
        public AccountWithdrawalEvent(Guid balanceId, Money amount) : base(balanceId, amount)
        {
        }
    }
}