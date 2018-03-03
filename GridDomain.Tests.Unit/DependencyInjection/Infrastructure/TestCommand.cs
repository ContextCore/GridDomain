using System;
using GridDomain.CQRS;

namespace GridDomain.Tests.Unit.DependencyInjection.Infrastructure {
    public class TestCommand : Command
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

    public class TestCommandB : Command
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