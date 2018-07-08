using GridDomain.Tests.Unit.CommandsExecution;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.Cluster.CommandsExecution
{
    public class Cluster_When_execute_command_Then_aggregate_Should_persist_changed : When_execute_command_Then_aggregate_Should_persist_changed
    {
        public Cluster_When_execute_command_Then_aggregate_Should_persist_changed(ITestOutputHelper output) 
            : base(new NodeTestFixture(output).Clustered()) {}
    }
}