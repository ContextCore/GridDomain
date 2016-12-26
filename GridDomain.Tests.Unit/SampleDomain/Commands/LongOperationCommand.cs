using System;
using GridDomain.CQRS;

namespace GridDomain.Tests.Unit.SampleDomain.Commands
{
    public class LongOperationCommand : Command
    {
        public LongOperationCommand(int parameter, Guid aggregateId)
        {
            Parameter = parameter;
            AggregateId = aggregateId;
        }

        public Guid AggregateId { get; }
        public int Parameter { get; }
    }
}