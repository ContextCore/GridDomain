using GridDomain.Tests.Acceptance.EventsUpgrade;
using GridDomain.Tests.Unit;
using GridDomain.Tests.Unit.Cluster;
using Xunit.Abstractions;

namespace GridDomain.Tests.Acceptance.Cluster.EventsUpgrade
{
    public class Cluster_GridNode_upgrade_events_in_subobject_by_json_adapters_when_loading_aggregate : GridNode_upgrade_events_in_subobject_by_json_adapters_when_loading_aggregate
    {
        public Cluster_GridNode_upgrade_events_in_subobject_by_json_adapters_when_loading_aggregate(ITestOutputHelper output)
            : base(ConfigureDomain(new BalloonFixture(output))
                       .Clustered()) { }
    }
}