using System;
using GridDomain.CQRS;

namespace GridDomain.Tests.SampleDomain
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