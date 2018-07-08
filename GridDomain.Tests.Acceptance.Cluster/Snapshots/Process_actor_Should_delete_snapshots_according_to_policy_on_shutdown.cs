using GridDomain.Tests.Acceptance.Snapshots;
using GridDomain.Tests.Unit.Cluster;
using GridDomain.Tests.Unit.ProcessManagers;
using Xunit.Abstractions;

namespace GridDomain.Tests.Acceptance.Cluster.Snapshots
{
    public class Cluster_Process_actor_Should_delete_snapshots_according_to_policy_on_shutdown : Process_actor_Should_delete_snapshots_according_to_policy_on_shutdown
    {
        public Cluster_Process_actor_Should_delete_snapshots_according_to_policy_on_shutdown(ITestOutputHelper output)
            : base(new SoftwareProgrammingProcessManagerFixture(output).Clustered()) { }
    }
}