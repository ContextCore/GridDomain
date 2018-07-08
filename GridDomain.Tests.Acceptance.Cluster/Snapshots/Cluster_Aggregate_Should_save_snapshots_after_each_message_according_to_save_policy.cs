using GridDomain.Tests.Acceptance.Snapshots;
using GridDomain.Tests.Unit;
using GridDomain.Tests.Unit.Cluster;
using Xunit.Abstractions;

namespace GridDomain.Tests.Acceptance.Cluster.Snapshots
{
    public class Cluster_Aggregate_Should_save_snapshots_after_each_message_according_to_save_policy : Aggregate_Should_save_snapshots_after_each_message_according_to_save_policy
    {
        public Cluster_Aggregate_Should_save_snapshots_after_each_message_according_to_save_policy(ITestOutputHelper output)
            : base(new BalloonFixture(output).UseSqlPersistence()
                                             .EnableSnapshots()
                                             .Clustered()) { }
    }
}