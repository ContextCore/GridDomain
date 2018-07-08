using GridDomain.Tests.Unit.FutureEvents;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.Cluster.FutureEvents
{
    public class Cluster_Raising_future_events_too_late : Raising_future_events_too_late
    {
        public Cluster_Raising_future_events_too_late(ITestOutputHelper output) :
            base(new FutureEventsFixture(output).Clustered()) {}
    }
}