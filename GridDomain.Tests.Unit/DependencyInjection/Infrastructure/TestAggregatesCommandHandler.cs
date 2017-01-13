using System;
using GridDomain.CQRS.Messaging.MessageRouting;
using Microsoft.Practices.Unity;

namespace GridDomain.Tests.Unit.DependencyInjection.Infrastructure
{
    public class TestAggregatesCommandHandler : AggregateCommandsHandler<TestAggregate>,
                                                        IAggregateCommandsHandlerDescriptor

    {
        private IUnityContainer _locator;

        //TODO: refactor to separate class
        public static readonly IAggregateCommandsHandlerDescriptor Descriptor = new TestAggregatesCommandHandler(null);
        public TestAggregatesCommandHandler(IUnityContainer unityContainer) : base()
        {
            _locator = unityContainer;
            Map<TestCommand>(c => c.AggregateId,
                            (c, a) => a.Execute(c.Parameter, _locator.Resolve<ITestDependency>()));
        }

        public Type AggregateType => typeof(TestAggregate);
    }
}