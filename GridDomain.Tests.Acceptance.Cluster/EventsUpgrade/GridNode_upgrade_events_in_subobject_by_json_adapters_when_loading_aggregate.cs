using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.EventSourcing.Adapters;
using GridDomain.Node.Configuration;
using GridDomain.Tests.Acceptance.Snapshots;
using GridDomain.Tests.Common;
using GridDomain.Tests.Unit;
using GridDomain.Tests.Unit.BalloonDomain;
using GridDomain.Tests.Unit.BalloonDomain.Commands;
using GridDomain.Tests.Unit.BalloonDomain.Events;
using GridDomain.Tests.Unit.Cluster;
using Serilog.Events;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Acceptance.EventsUpgrade
{
    public class Cluster_GridNode_upgrade_events_in_subobject_by_json_adapters_when_loading_aggregate : GridNode_upgrade_events_in_subobject_by_json_adapters_when_loading_aggregate
    {
        public Cluster_GridNode_upgrade_events_in_subobject_by_json_adapters_when_loading_aggregate(ITestOutputHelper output)
            : base(ConfigureDomain(new BalloonFixture(output))
                       .Clustered()) { }
    }
}