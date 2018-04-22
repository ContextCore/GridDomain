using GridDomain.Tests.Unit.ProcessManagers;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.Cluster.ProcessManagers
{
    public class ClusterProcessManagerProducedEventsAndCommandsHasProcessId : ProcessManagerProducedEventsAndCommandsHasProcessId
    {
        public ClusterProcessManagerProducedEventsAndCommandsHasProcessId(ITestOutputHelper helper)
            : base(new SoftwareProgrammingProcessManagerFixture(helper).Clustered()) { }
    }
}