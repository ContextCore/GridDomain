using GridDomain.Tests.Unit.CommandsExecution;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.Cluster.CommandsExecution
{
    public class Cluster_Execute_command_until_projection_build_notification_fetching_results : Execute_command_until_projection_build_notification_fetching_results
    {
        public Cluster_Execute_command_until_projection_build_notification_fetching_results(ITestOutputHelper output) : 
            base(new NodeTestFixture(output).Clustered().LogToFile(nameof(Cluster_Execute_command_until_projection_build_notification_fetching_results))) {}
    }
}