using System;
using GridDomain.CQRS;

namespace GridDomain.Tests.SampleDomain.Commands
{
    public class CreateAggregateCommand : Command
    {
        public CreateAggregateCommand(int parameter, Guid aggregateId)
        {
            Parameter = parameter;
            AggregateId = aggregateId;
        }

        public Guid AggregateId { get; }
        public int Parameter { get; }
    }
}