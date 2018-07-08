using GridDomain.Tests.Acceptance.FutureDomainEvents;
using GridDomain.Tests.Unit.Cluster;
using GridDomain.Tests.Unit.FutureEvents;
using Xunit.Abstractions;

namespace GridDomain.Tests.Acceptance.Cluster.FutureDomainEvents
{
    public class Cluster_Given_aggregate_When_raising_several_future_events_Persistent : Given_aggregate_When_raising_several_future_events_Persistent
    {
        public Cluster_Given_aggregate_When_raising_several_future_events_Persistent(ITestOutputHelper output) 
            : base(new FutureEventsFixture(output).Clustered()) { }

    }
}