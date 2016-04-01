using System;
using System.Collections.Generic;
using GridDomain.Balance.Commands;
using NMoneys;

namespace GridDomain.Tests.Acceptance
{
    public class BalanceChangePlan
    {
        public IReadOnlyCollection<ChangeBalanceCommand> BalanceChangeCommands;
        public CreateBalanceCommand BalanceCreateCommand;
        public Guid balanceId;
        public Guid businessId;
        public Money TotalAmountChange;
        public Money TotalReplenish;
        public Money TotalWithdrwal;
    }
}