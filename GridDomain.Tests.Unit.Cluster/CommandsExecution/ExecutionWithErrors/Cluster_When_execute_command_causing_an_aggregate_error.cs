using GridDomain.Tests.Unit.CommandsExecution.ExecutionWithErrors;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.Cluster.CommandsExecution.ExecutionWithErrors
{
    public class Cluster_When_execute_command_causing_an_aggregate_error : When_execute_command_causing_an_aggregate_error
    {
        public Cluster_When_execute_command_causing_an_aggregate_error(ITestOutputHelper output)
            : base(new NodeTestFixture(output).Clustered()) {}
    }
}