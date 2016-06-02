using System;
using NMoneys;

namespace GridDomain.Balance.Domain.BalanceAggregate.Commands
{
    public class ReplenishAccountCommand : ChangeAccountCommand
    {
        public ReplenishAccountCommand(Guid balanceId, Money amount, AccountChangeSource changeSource)
            : base(balanceId, amount, changeSource)
        {
        }
    }
}