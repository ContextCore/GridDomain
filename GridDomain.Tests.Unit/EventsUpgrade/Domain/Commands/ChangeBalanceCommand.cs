using System;
using GridDomain.CQRS;
using GridDomain.ProcessManagers;

namespace GridDomain.Tests.Unit.EventsUpgrade.Domain.Commands
{
    public class ChangeBalanceCommand : Command<BalanceAggregate>
    {
        public ChangeBalanceCommand(int parameter, string aggregateId) : base(aggregateId)
        {
            Parameter = parameter;
        }

        public int Parameter { get; }
    }
}