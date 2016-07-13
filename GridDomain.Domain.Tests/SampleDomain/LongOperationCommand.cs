using System;
using GridDomain.CQRS;

namespace GridDomain.Tests.SampleDomain
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

    public class AlwaysFaultCommand : Command
    {
        public AlwaysFaultCommand(Guid aggregateId)
        {
            AggregateId = aggregateId;
        }

        public Guid AggregateId { get; }
    }
}