using System;
using GridDomain.CQRS;

namespace GridDomain.Tests.XUnit.SampleDomain.Commands
{
    public class ChangeSampleAggregateCommand : Command
    {
        public ChangeSampleAggregateCommand(int parameter, Guid aggregateId) : base(aggregateId)
        {
            Parameter = parameter;
        }

        public int Parameter { get; }
    }
}