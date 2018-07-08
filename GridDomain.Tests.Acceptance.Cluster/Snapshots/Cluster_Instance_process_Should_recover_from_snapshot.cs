using GridDomain.Tests.Acceptance.Snapshots;
using GridDomain.Tests.Unit.Cluster;
using GridDomain.Tests.Unit.ProcessManagers;
using Xunit.Abstractions;

namespace GridDomain.Tests.Acceptance.Cluster.Snapshots
{
    public class Cluster_Instance_process_Should_recover_from_snapshot : Instance_process_Should_recover_from_snapshot
    {
        public Cluster_Instance_process_Should_recover_from_snapshot(ITestOutputHelper helper)
            : base(new SoftwareProgrammingProcessManagerFixture(helper).UseSqlPersistence().Clustered()) {}
    }
}