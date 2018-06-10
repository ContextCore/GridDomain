using System;
using System.Linq;
using System.Threading.Tasks;
using GridDomain.Tests.Acceptance.EventsUpgrade.SampleDomain;
using GridDomain.Tests.Acceptance.Snapshots;
using GridDomain.Tests.Common;
using GridDomain.Tests.Unit;
using GridDomain.Tests.Unit.BalloonDomain.Configuration;
using GridDomain.Tests.Unit.Cluster;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Acceptance.EventsUpgrade
{
    public class Cluster_GridNode_should_upgrade_objects_in_domain_events_after_save_load_by_journal : GridNode_should_upgrade_objects_in_domain_events_after_save_load_by_journal
    {
        public Cluster_GridNode_should_upgrade_objects_in_domain_events_after_save_load_by_journal(ITestOutputHelper output)
            : base(ConfigureDomain(new NodeTestFixture(output)).Clustered()) {}

    }
}