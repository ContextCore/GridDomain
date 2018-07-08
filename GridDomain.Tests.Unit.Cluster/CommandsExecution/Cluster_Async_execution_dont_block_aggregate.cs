using GridDomain.Tests.Unit.CommandsExecution;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.Cluster.CommandsExecution
{

    public class Cluster_Async_execution_dont_block_aggregate : Async_execution_dont_block_aggregate
    {
        public Cluster_Async_execution_dont_block_aggregate(ITestOutputHelper output) : base(new NodeTestFixture(output).Clustered()) {}
    }
}