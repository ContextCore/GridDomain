using GridDomain.Tests.Unit.CommandsExecution;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.Cluster.CommandsExecution {
    public class Cluster_When_awaiting_command_execution_without_prepare_and_counting : When_awaiting_command_execution_without_prepare_and_counting
    {
        public Cluster_When_awaiting_command_execution_without_prepare_and_counting(ITestOutputHelper output) :
            base(new NodeTestFixture(output).Clustered()){ }
    }
}