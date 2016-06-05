using System;
using GridDomain.CQRS;
using NMoneys;

namespace GridDomain.Balance.Domain.AccountAggregate.Commands
{
    public class ChangeAccountCommand : Command
    {
        public ChangeAccountCommand(Guid balanceId,Money amount)
        {
            BalanceId = balanceId;
            Amount = amount;
        }

        public Guid BalanceId { get; private set; }
        public Money Amount { get; private set; }
    }
}