using System;
using GridDomain.CQRS.Messaging.MessageRouting;
using GridDomain.EventSourcing;
using Microsoft.Practices.Unity;

namespace GridDomain.Tests.Unit.DependencyInjection.Infrastructure
{
    public class TestAggregatesCommandHandler : AggregateCommandsHandler<TestAggregate>,
                                                IAggregateCommandsHandlerDescriptor

    {
        //TODO: refactor to separate class
        public static readonly IAggregateCommandsHandlerDescriptor Descriptor = new TestAggregatesCommandHandler(null);

        public TestAggregatesCommandHandler(ITestDependency testDependency)
        {
            Map<TestCommand>((c, a) => a.Execute(c.Parameter, testDependency));
        }

        public Type AggregateType => typeof(TestAggregate);
    }
}