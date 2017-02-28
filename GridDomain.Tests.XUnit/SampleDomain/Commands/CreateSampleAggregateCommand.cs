using System;
using GridDomain.CQRS;

namespace GridDomain.Tests.XUnit.SampleDomain.Commands
{
    public class CreateSampleAggregateCommand : Command
    {
        public CreateSampleAggregateCommand(int parameter, Guid aggregateId) : base(aggregateId)
        {
            Parameter = parameter;
        }

        public int Parameter { get; }
    }
}