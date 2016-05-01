using System;
using GridDomain.Balance.Domain;
using GridDomain.CQRS;
using NMoneys;

namespace GridDomain.Balance.Commands
{
    public class ChangeBalanceCommand : Command
    {
        public ChangeBalanceCommand(Guid balanceId,
            Money amount,
            BalanceChangeSource changeSource)
        {
            BalanceId = balanceId;
            Amount = amount;
            ChangeSource = changeSource;
        }

        public Guid BalanceId { get; private set; }
        public Money Amount { get; private set; }
        public BalanceChangeSource ChangeSource { get; private set; }
    }
}