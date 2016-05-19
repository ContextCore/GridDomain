using System;
using System.Collections.Generic;
using GridDomain.Balance.Domain.BalanceAggregate.Commands;
using GridDomain.CQRS;
using NMoneys;

namespace GridDomain.Tests.Acceptance
{
    public class BalanceChangePlan
    {
        public Guid businessId;
        public Guid BalanceId;
        public IReadOnlyCollection<ChangeBalanceCommand> BalanceChangeCommands;
        public CreateBalanceCommand BalanceCreateCommand;
        public Money TotalAmountChange;
        public Money TotalReplenish;
        public Money TotalWithdrwal;
    }
}