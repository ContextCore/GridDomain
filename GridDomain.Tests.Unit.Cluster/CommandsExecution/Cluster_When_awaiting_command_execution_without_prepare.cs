using GridDomain.Tests.Unit.CommandsExecution;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.Cluster.CommandsExecution
{
    public class Cluster_When_awaiting_command_execution_without_prepare : When_awaiting_command_execution_without_prepare
    {
        public Cluster_When_awaiting_command_execution_without_prepare(ITestOutputHelper output) 
            : base(new NodeTestFixture(output).Clustered()) { }
    }
}
