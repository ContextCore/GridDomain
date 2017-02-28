using System;
using GridDomain.CQRS.Messaging.MessageRouting;
using Microsoft.Practices.Unity;

namespace GridDomain.Tests.XUnit.DependencyInjection.Infrastructure
{
    public class TestAggregatesCommandHandler : AggregateCommandsHandler<TestAggregate>,
                                                IAggregateCommandsHandlerDescriptor

    {
        //TODO: refactor to separate class
        public static readonly IAggregateCommandsHandlerDescriptor Descriptor = new TestAggregatesCommandHandler(null);
        private readonly IUnityContainer _locator;

        public TestAggregatesCommandHandler(IUnityContainer unityContainer)
        {
            _locator = unityContainer;
            Map<TestCommand>((c, a) => a.Execute(c.Parameter, _locator.Resolve<ITestDependency>()));
        }

        public Type AggregateType => typeof(TestAggregate);
    }
}