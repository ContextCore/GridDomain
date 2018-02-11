using System;
using GridDomain.CQRS;

namespace GridDomain.Tests.Unit.EventsUpgrade.Domain.Commands
{
    public class ChangeBalanceCommand : Command
    {
        public ChangeBalanceCommand(int parameter, string aggregateId) : base(aggregateId)
        {
            Parameter = parameter;
        }

        public int Parameter { get; }
    }
}