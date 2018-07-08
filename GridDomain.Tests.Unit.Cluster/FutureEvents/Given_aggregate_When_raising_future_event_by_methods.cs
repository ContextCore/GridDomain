using GridDomain.Tests.Unit.FutureEvents;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.Cluster.FutureEvents
{
    public class Cluster_Given_aggregate_When_raising_future_event_by_methods : Given_aggregate_When_raising_future_event_by_methods
    {
        public Cluster_Given_aggregate_When_raising_future_event_by_methods(ITestOutputHelper output)
            : base(new FutureEventsFixture(output).Clustered()) { }
       
    }
}