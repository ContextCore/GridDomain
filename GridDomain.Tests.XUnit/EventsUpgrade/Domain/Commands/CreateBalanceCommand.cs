using System;
using GridDomain.CQRS;

namespace GridDomain.Tests.XUnit.EventsUpgrade.Domain.Commands
{
    public class CreateBalanceCommand : Command
    {
        public CreateBalanceCommand(int parameter, Guid aggregateId)
        {
            Parameter = parameter;
            AggregateId = aggregateId;
        }

        public Guid AggregateId { get; }
        public int Parameter { get; }
    }
}