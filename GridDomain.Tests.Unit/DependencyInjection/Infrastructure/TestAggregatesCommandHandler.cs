using System;
using GridDomain.EventSourcing;

namespace GridDomain.Tests.Unit.DependencyInjection.Infrastructure
{
    public class TestAggregatesCommandHandler : AggregateCommandsHandler<TestAggregate>

    {
        public TestAggregatesCommandHandler(ITestDependency testDependency)
        {
            Map<TestCommand>((c, a) => a.Execute(c.Parameter, testDependency));
        }

    }
}