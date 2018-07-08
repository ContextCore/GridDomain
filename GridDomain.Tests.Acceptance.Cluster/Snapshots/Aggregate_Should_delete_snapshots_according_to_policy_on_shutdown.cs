using GridDomain.Tests.Acceptance.Snapshots;
using GridDomain.Tests.Unit;
using GridDomain.Tests.Unit.Cluster;
using Xunit.Abstractions;

namespace GridDomain.Tests.Acceptance.Cluster.Snapshots
{
    public class Cluster_Aggregate_Should_delete_snapshots_according_to_policy_on_shutdown : Aggregate_Should_delete_snapshots_according_to_policy_on_shutdown
    {
        public Cluster_Aggregate_Should_delete_snapshots_according_to_policy_on_shutdown(ITestOutputHelper output)
            : base(ConfigureDomain(new BalloonFixture(output))
                       .Clustered()) { }
    }
}