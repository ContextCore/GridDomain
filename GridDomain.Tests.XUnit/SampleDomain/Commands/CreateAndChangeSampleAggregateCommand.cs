using System;
using GridDomain.CQRS;

namespace GridDomain.Tests.XUnit.SampleDomain.Commands
{
    public class CreateAndChangeSampleAggregateCommand : Command
    {
        public CreateAndChangeSampleAggregateCommand(int parameter, Guid aggregateId) : base(aggregateId)
        {
            Parameter = parameter;
        }

        public int Parameter { get; }
    }
}