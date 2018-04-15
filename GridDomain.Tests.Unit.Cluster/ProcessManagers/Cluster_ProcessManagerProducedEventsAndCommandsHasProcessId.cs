using GridDomain.Tests.Unit.ProcessManagers;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.Cluster.ProcessManagers
{
    public class Cluster_ProcessManagerProducedEventsAndCommandsHasProcessId : ProcessManagerProducedEventsAndCommandsHasProcessId
    {
        public Cluster_ProcessManagerProducedEventsAndCommandsHasProcessId(ITestOutputHelper helper)
            : base(new SoftwareProgrammingProcessManagerFixture(helper).Clustered()) { }
    }
}