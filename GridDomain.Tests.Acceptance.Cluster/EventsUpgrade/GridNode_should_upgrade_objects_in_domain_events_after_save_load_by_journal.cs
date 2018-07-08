using GridDomain.Tests.Acceptance.EventsUpgrade;
using GridDomain.Tests.Unit;
using GridDomain.Tests.Unit.Cluster;
using Xunit.Abstractions;

namespace GridDomain.Tests.Acceptance.Cluster.EventsUpgrade
{
    public class Cluster_GridNode_should_upgrade_objects_in_domain_events_after_save_load_by_journal : GridNode_should_upgrade_objects_in_domain_events_after_save_load_by_journal
    {
        public Cluster_GridNode_should_upgrade_objects_in_domain_events_after_save_load_by_journal(ITestOutputHelper output)
            : base(ConfigureDomain(new NodeTestFixture(output)).Clustered()) {}

    }
}