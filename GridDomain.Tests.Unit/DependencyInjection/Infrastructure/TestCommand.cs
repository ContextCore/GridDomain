using System;
using GridDomain.CQRS;

namespace GridDomain.Tests.XUnit.DependencyInjection.Infrastructure
{
    public class TestCommand : Command
    {
        public TestCommand(int parameter, Guid aggregateId) : base(aggregateId)
        {
            Parameter = parameter;
        }

        public int Parameter { get; }
    }
}