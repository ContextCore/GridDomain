using System;
using NMoneys;

namespace GridDomain.Balance.Domain.BalanceAggregate.Commands
{
    public class WithdrawalBalanceCommand : ChangeBalanceCommand
    {
        public WithdrawalBalanceCommand(Guid balanceId, Money amount, BalanceChangeSource changeSource)
            : base(balanceId, amount, changeSource)
        {
        }
    }
}