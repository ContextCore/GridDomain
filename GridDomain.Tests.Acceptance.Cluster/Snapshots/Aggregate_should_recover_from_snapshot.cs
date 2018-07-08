using GridDomain.Tests.Acceptance.Snapshots;
using GridDomain.Tests.Unit;
using GridDomain.Tests.Unit.Cluster;
using Xunit.Abstractions;

namespace GridDomain.Tests.Acceptance.Cluster.Snapshots
{
    public class Cluster_Aggregate_should_recover_from_snapshot : Aggregate_should_recover_from_snapshot
    {
        public Cluster_Aggregate_should_recover_from_snapshot(ITestOutputHelper output)
            : base(ConfigureFixture(new BalloonFixture(output))
                       .Clustered()) { }
    }
}