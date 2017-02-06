using System;
using GridDomain.Common;
using GridDomain.CQRS.Messaging;
using GridDomain.Node;
using GridDomain.Node.Configuration.Akka;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Scheduling.Quartz;
using GridDomain.Tests.Framework;
using GridDomain.Tests.Framework.Configuration;
using GridDomain.Tests.XUnit.DependencyInjection.Infrastructure;
using Microsoft.Practices.Unity;
using Xunit.Abstractions;

namespace GridDomain.Tests.XUnit.DependencyInjection
{
    public class AggregatesDI : NodeTestKit
    {
        public AggregatesDI(ITestOutputHelper helper) : base(helper, new DINodeFixture())
        {

        }

        class DINodeFixture : NodeTestFixture
        {
            public DINodeFixture()
            {
                Add(new CustomContainerConfiguration(c =>
                                                     {
                                                         c.RegisterType<ITestDependency, TestDependencyImplementation>();
                                                         c.RegisterInstance<IQuartzConfig>(new InMemoryQuartzConfig());
                                                         c.RegisterAggregate<TestAggregate, TestAggregatesCommandHandler>();
                                                     }));
                Add(new TestRouteMap());
            }
        }
    }
}
