using GridDomain.Tests.Acceptance.EventsUpgrade;
using GridDomain.Tests.Unit.Cluster;
using Xunit.Abstractions;

namespace GridDomain.Tests.Acceptance.Cluster.EventsUpgrade
{
    public class Cluster_Future_events_class_upgraded_by_object_adapter : Future_events_class_upgraded_by_object_adapter
    {

        public Cluster_Future_events_class_upgraded_by_object_adapter(ITestOutputHelper output)
            : base(CreateFixture(output).Clustered()) { }
    }
}