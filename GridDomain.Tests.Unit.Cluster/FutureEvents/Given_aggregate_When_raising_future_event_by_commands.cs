using GridDomain.Tests.Unit.FutureEvents;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.Cluster.FutureEvents
{
    public class Cluster_Given_aggregate_When_raising_future_event_by_commands : Given_aggregate_When_raising_future_event_by_commands
    {
        public Cluster_Given_aggregate_When_raising_future_event_by_commands(ITestOutputHelper output)
            : base(new FutureEventsFixture(output).Clustered()) { }
    }
}