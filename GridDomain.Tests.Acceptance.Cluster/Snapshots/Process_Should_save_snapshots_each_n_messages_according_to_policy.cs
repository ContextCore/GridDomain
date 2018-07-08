using GridDomain.Tests.Acceptance.Snapshots;
using GridDomain.Tests.Unit.Cluster;
using GridDomain.Tests.Unit.ProcessManagers;
using Xunit.Abstractions;

namespace GridDomain.Tests.Acceptance.Cluster.Snapshots
{
    public class Cluster_Process_Should_save_snapshots_each_n_messages_according_to_policy : Process_Should_save_snapshots_each_n_messages_according_to_policy
    {
        public Cluster_Process_Should_save_snapshots_each_n_messages_according_to_policy(ITestOutputHelper output)
            : base(ConfigureFixture(new SoftwareProgrammingProcessManagerFixture(output)).Clustered()){}
    }
}