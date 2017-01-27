using System;
using GridDomain.CQRS;

namespace GridDomain.Tests.XUnit.EventsUpgrade.Domain.Commands
{
    public class ChangeBalanceCommand : Command
    {
        public ChangeBalanceCommand(int parameter, Guid aggregateId)
        {
            Parameter = parameter;
            AggregateId = aggregateId;
        }

        public Guid AggregateId { get; }
        public int Parameter { get; }
    }
}