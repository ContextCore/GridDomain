using System;
using NMoneys;

namespace GridDomain.Balance.Domain.AccountAggregate.Commands
{
    public class ReplenishAccountByCardCommand : ChangeAccountCommand
    {
        public ReplenishAccountByCardCommand(Guid balanceId, Money amount)
            : base(balanceId, amount)
        {
        }
    }
}