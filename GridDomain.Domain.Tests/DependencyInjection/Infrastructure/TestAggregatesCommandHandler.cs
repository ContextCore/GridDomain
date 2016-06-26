using System;
using GridDomain.CQRS.Messaging;
using GridDomain.CQRS.Messaging.MessageRouting;

namespace GridDomain.Tests.DependencyInjection.Infrastructure
{
    public class TestAggregatesCommandHandler : AggregateCommandsHandler<TestAggregate>,
                                                        IAggregateCommandsHandlerDesriptor

    {
        private IServiceLocator _locator;

        //TODO: refactor to separate class
        public static readonly IAggregateCommandsHandlerDesriptor Descriptor = new TestAggregatesCommandHandler(null);
        public TestAggregatesCommandHandler(IServiceLocator serviceLocator) : base(serviceLocator)
        {
            _locator = serviceLocator;
            Map<TestCommand>(c => c.AggregateId,
                            (c, a) => a.Execute(c.Parameter, _locator.Resolve<ITestDependency>()));
        }

        public Type AggregateType => typeof(TestAggregate);
    }
}