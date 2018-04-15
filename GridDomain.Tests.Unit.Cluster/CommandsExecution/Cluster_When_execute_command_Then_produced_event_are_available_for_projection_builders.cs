using GridDomain.Tests.Unit.CommandsExecution;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.Cluster.CommandsExecution
{
    public class Cluster_When_execute_command_Then_produced_event_are_available_for_projection_builders : When_execute_command_Then_produced_event_are_available_for_projection_builders
    {
        public Cluster_When_execute_command_Then_produced_event_are_available_for_projection_builders(ITestOutputHelper output) 
            : base(new NodeTestFixture(output).Clustered()) { }
    }
}