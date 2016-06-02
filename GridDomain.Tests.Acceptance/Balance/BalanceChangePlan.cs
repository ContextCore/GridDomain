using System;
using System.Collections.Generic;
using GridDomain.Balance.Domain.BalanceAggregate.Commands;
using NMoneys;

namespace GridDomain.Tests.Acceptance.Balance
{
    public class BalanceChangePlan
    {
        public IReadOnlyCollection<ChangeAccountCommand> BalanceChangeCommands;
        public CreateAccountCommand AccountCreateCommand;
        public Guid BalanceId;
        public Guid businessId;
        public Money TotalAmountChange;
        public Money TotalReplenish;
        public Money TotalWithdrwal;
    }
}