using System;
using GridDomain.CQRS;

namespace GridDomain.Tests.XUnit.SampleDomain.Commands
{
    public class ChangeSampleAggregateCommand : Command
    {
        public ChangeSampleAggregateCommand(int parameter, Guid aggregateId)
        {
            Parameter = parameter;
            AggregateId = aggregateId;
        }

        public Guid AggregateId { get; }
        public int Parameter { get; }
    }
}