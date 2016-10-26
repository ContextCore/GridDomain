using System;
using GridDomain.CQRS;

namespace GridDomain.Tests.SampleDomain.Commands
{
    public class CreateSampleAggregateCommand : Command
    {
        public CreateSampleAggregateCommand(int parameter, Guid aggregateId, Guid commandId):base(commandId)
        {
            Parameter = parameter;
            AggregateId = aggregateId;
        }

        public CreateSampleAggregateCommand(int parameter, Guid aggregateId)
        {
            Parameter = parameter;
            AggregateId = aggregateId;
        }

        public Guid AggregateId { get; }
        public int Parameter { get; }
    }
}