using System;
using GridDomain.CQRS;

namespace GridDomain.Tests.XUnit.SampleDomain.Commands
{
    public class CreateAndChangeSampleAggregateCommand : Command
    {
        public CreateAndChangeSampleAggregateCommand(int parameter, Guid aggregateId)
        {
            Parameter = parameter;
            AggregateId = aggregateId;
        }

        public Guid AggregateId { get; }
        public int Parameter { get; }
    }
}