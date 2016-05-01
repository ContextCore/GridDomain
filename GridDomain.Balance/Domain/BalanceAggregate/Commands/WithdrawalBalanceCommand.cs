using System;
using GridDomain.Balance.Domain;
using NMoneys;

namespace GridDomain.Balance.Commands
{
    public class WithdrawalBalanceCommand : ChangeBalanceCommand
    {
        public WithdrawalBalanceCommand(Guid balanceId, Money amount, BalanceChangeSource changeSource)
            : base(balanceId, amount, changeSource)
        {
        }
    }
}