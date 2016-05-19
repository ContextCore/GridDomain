using System;
using NMoneys;

namespace GridDomain.Balance.Domain.BalanceAggregate.Commands
{
    public class ReplenishBalanceCommand : ChangeBalanceCommand
    {
        public ReplenishBalanceCommand(Guid balanceId, Money amount, BalanceChangeSource changeSource)
            : base(balanceId, amount, changeSource)
        {
        }
    }
}