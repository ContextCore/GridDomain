using System;
using GridDomain.CQRS;
using GridDomain.ProcessManagers;

namespace GridDomain.Tests.Unit.DependencyInjection.Infrastructure {
    public class TestCommand : Command<TestAggregate>
    {
        public TestCommand(int parameter, Guid aggregateId) : this(parameter,aggregateId.ToString())
        {
        } 
        public TestCommand(int parameter, string aggregateId) : base(aggregateId)
        {
            Parameter = parameter;
        }

        public int Parameter { get; }
    }
}