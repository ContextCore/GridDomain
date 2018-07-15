using GridDomain.Tests.Unit.CommandsExecution;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.Cluster.CommandsExecution
{
    public class Cluster_Execute_command_waiting_aggregate_event : Execute_command_waiting_aggregate_event
    {
        public Cluster_Execute_command_waiting_aggregate_event(ITestOutputHelper output) :
            base(new NodeTestFixture(output).Clustered().LogToFile(nameof(Cluster_Execute_command_waiting_aggregate_event))) {}
    }
}