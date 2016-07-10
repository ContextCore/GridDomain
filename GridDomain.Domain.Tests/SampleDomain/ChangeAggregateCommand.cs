using System;
using GridDomain.CQRS;

namespace GridDomain.Tests.SampleDomain
{
    public class ChangeAggregateCommand : Command
    {
        public ChangeAggregateCommand(int parameter, Guid aggregateId)
        {
            Parameter = parameter;
            AggregateId = aggregateId;
        }

        public Guid AggregateId { get; }
        public int Parameter { get; }
    }

    public class ExternalCallCommand : Command
    {
        public ExternalCallCommand(int parameter, Guid aggregateId)
        {
            Parameter = parameter;
            AggregateId = aggregateId;
        }

        public Guid AggregateId { get; }
        public int Parameter { get; }
    }

}