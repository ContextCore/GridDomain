using GridDomain.Tests.Acceptance.Snapshots;
using GridDomain.Tests.Unit.Cluster;
using GridDomain.Tests.Unit.ProcessManagers;
using Xunit.Abstractions;

namespace GridDomain.Tests.Acceptance.Cluster.Snapshots
{
    public class Cluster_InstanceProcessShouldNotSaveSnapshotsOnMessageProcessByDefault : InstanceProcessShouldNotSaveSnapshotsOnMessageProcessByDefault
    {
        public Cluster_InstanceProcessShouldNotSaveSnapshotsOnMessageProcessByDefault(ITestOutputHelper output)
            : base(new SoftwareProgrammingProcessManagerFixture(output).Clustered()) { }
    }
}