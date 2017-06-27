﻿using GridDomain.Node.Configuration.Composition;
using GridDomain.Scheduling.Quartz;
using GridDomain.Tests.Unit.DependencyInjection.Infrastructure;
using Microsoft.Practices.Unity;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.DependencyInjection
{
    public class AggregatesDI : NodeTestKit
    {
        public AggregatesDI(ITestOutputHelper helper) : base(helper, new DINodeFixture()) {}

        private class DINodeFixture : NodeTestFixture
        {
            public DINodeFixture()
            {
                Add(new ContainerConfiguration(c =>
                                                     {
                                                         c.RegisterType<ITestDependency, TestDependencyImplementation>();
                                                         c.RegisterInstance<IQuartzConfig>(new InMemoryQuartzConfig());
                                                         c.Register(AggregateConfiguration.New<TestAggregate, TestAggregatesCommandHandler>());
                                                     }));
                Add(new TestRouteMap());
            }
        }
    }
}