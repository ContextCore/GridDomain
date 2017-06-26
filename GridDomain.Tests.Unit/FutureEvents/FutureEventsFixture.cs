using GridDomain.Node.Configuration.Composition;
using GridDomain.Tests.XUnit.FutureEvents.Infrastructure;
using Xunit.Abstractions;

namespace GridDomain.Tests.XUnit.FutureEvents
{
    internal class FutureEventsFixture : NodeTestFixture
    {
        public FutureEventsFixture(ITestOutputHelper output = null) : base(null, null, null, output)
        {
            Add(new CustomContainerConfiguration(c => c.RegisterAggregate<FutureEventsAggregate, FutureEventsAggregatesCommandHandler>()));
            Add(new FutureEventsRouteMap());
            this.ClearSheduledJobs();
        }
    }
}