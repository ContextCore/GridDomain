using GridDomain.Node.Actors;
using GridDomain.Node.Configuration.Composition;
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
                Add(new TestAggregateDomainConfiguration());
                Add(new TestRouteMap());
            }
        }
    }
}