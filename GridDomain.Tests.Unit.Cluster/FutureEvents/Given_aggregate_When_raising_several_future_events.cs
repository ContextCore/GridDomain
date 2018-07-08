using GridDomain.Tests.Unit.FutureEvents;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.Cluster.FutureEvents
{
    public class Cluster_Given_aggregate_When_raising_several_future_events : Given_aggregate_When_raising_several_future_events
    {
        public Cluster_Given_aggregate_When_raising_several_future_events(ITestOutputHelper output)
            : base(new FutureEventsFixture(output).Clustered()) { }
       
    }
}