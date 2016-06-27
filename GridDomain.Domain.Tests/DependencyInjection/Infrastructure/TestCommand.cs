using System;
using GridDomain.CQRS;

namespace GridDomain.Tests.DependencyInjection.Infrastructure
{
    public class TestCommand : Command
    {
        public TestCommand(int parameter, Guid aggregateId)
        {
            Parameter = parameter;
            AggregateId = aggregateId;
        }

        public Guid AggregateId { get; }
        public int Parameter { get; }
    }
}