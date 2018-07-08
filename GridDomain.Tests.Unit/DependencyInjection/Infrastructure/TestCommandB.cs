using System;
using GridDomain.ProcessManagers;

namespace GridDomain.Tests.Unit.DependencyInjection.Infrastructure {
    public class TestCommandB : Command<TestAggregate>
    {
        public TestCommandB(int parameter, Guid aggregateId) : this(parameter,aggregateId.ToString())
        {
        }
    
        public TestCommandB(int parameter, string aggregateId) : base(aggregateId)
        {
            Parameter = parameter;
        }

        public int Parameter { get; }
    }
}