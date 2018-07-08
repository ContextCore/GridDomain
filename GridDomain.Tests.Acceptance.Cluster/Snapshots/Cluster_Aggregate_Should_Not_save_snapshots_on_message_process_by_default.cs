using GridDomain.Tests.Acceptance.Snapshots;
using GridDomain.Tests.Unit;
using GridDomain.Tests.Unit.Cluster;
using Xunit.Abstractions;

namespace GridDomain.Tests.Acceptance.Cluster.Snapshots
{
    public class Cluster_Aggregate_Should_Not_save_snapshots_on_message_process_by_default : Aggregate_Should_Not_save_snapshots_on_message_process_by_default
    {
        public Cluster_Aggregate_Should_Not_save_snapshots_on_message_process_by_default(ITestOutputHelper output)
            : base(new BalloonFixture(output).Clustered()) {}

    }
}