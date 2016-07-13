using System;
using GridDomain.CQRS.Messaging;
using GridDomain.CQRS.Messaging.MessageRouting;
using Microsoft.Practices.Unity;

namespace GridDomain.Tests.DependencyInjection.Infrastructure
{
    public class TestAggregatesCommandHandler : AggregateCommandsHandler<TestAggregate>,
                                                        IAggregateCommandsHandlerDesriptor

    {
        private IUnityContainer _locator;

        //TODO: refactor to separate class
        public static readonly IAggregateCommandsHandlerDesriptor Descriptor = new TestAggregatesCommandHandler(null);
        public TestAggregatesCommandHandler(IUnityContainer unityContainer) : base(unityContainer)
        {
            _locator = unityContainer;
            Map<TestCommand>(c => c.AggregateId,
                            (c, a) => a.Execute(c.Parameter, _locator.Resolve<ITestDependency>()));
        }

        public Type AggregateType => typeof(TestAggregate);
    }
}