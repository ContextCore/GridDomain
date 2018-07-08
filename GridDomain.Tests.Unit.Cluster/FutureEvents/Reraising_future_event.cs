using GridDomain.Tests.Unit.FutureEvents;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.Cluster.FutureEvents
{
    public class Cluster_Reraising_future_event : Reraising_future_event
    {
        public Cluster_Reraising_future_event(ITestOutputHelper output) 
           : base(new FutureEventsFixture(output).Clustered()) {}
    }
}