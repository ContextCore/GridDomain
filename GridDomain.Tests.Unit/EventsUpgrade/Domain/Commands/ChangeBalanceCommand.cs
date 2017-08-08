using System;
using GridDomain.CQRS;

namespace GridDomain.Tests.Unit.EventsUpgrade.Domain.Commands
{
    public class ChangeBalanceCommand : Command
    {
        public ChangeBalanceCommand(int parameter, Guid aggregateId) : base(aggregateId)
        {
            Parameter = parameter;
        }

        public int Parameter { get; }
    }
}