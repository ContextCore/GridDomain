using System;
using GridDomain.Balance.Domain;
using NMoneys;

namespace GridDomain.Balance.Commands
{
    public class ReplenishBalanceCommand : ChangeBalanceCommand
    {
        public ReplenishBalanceCommand(Guid balanceId, Money amount, BalanceChangeSource changeSource)
            : base(balanceId, amount, changeSource)
        {
        }
    }
}