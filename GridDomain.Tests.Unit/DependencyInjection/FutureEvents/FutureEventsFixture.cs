using GridDomain.Node.Configuration.Composition;
using GridDomain.Tests.Unit.DependencyInjection.FutureEvents.Configuration;
using GridDomain.Tests.Unit.FutureEvents.Infrastructure;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.FutureEvents
{
    internal class FutureEventsFixture : NodeTestFixture
    {
        public FutureEventsFixture(ITestOutputHelper output = null) : base(null, null, null, output)
        {
            Add(new FutureAggregateDomainConfiguration());
            Add(new FutureEventsRouteMap());
            this.ClearSheduledJobs();
        }
    }
}