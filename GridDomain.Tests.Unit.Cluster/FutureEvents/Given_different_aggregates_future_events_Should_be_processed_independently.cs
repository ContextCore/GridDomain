using GridDomain.Tests.Unit.FutureEvents;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.Cluster.FutureEvents
{
    public class Cluster_Given_different_aggregates_future_events_Should_be_processed_independently : Given_different_aggregates_future_events_Should_be_processed_independently
    {
        public Cluster_Given_different_aggregates_future_events_Should_be_processed_independently(ITestOutputHelper output)
            : base(new FutureEventsFixture(output).Clustered()) { }
    }
}