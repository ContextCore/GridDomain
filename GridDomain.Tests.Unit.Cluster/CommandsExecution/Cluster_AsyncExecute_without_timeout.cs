using GridDomain.Tests.Unit.CommandsExecution;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.Cluster.CommandsExecution
{
    public class Cluster_AsyncExecute_without_timeout : AsyncExecute_without_timeout
    {
        public Cluster_AsyncExecute_without_timeout(ITestOutputHelper output) : base(new NodeTestFixture(output).Clustered()) {}
    }
}