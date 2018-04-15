using GridDomain.Tests.Unit.CommandsExecution;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.Cluster.CommandsExecution
{
    public class Cluster_SyncExecute_with_timeout : SyncExecute_with_timeout
    {
        public Cluster_SyncExecute_with_timeout(ITestOutputHelper output) : base(new NodeTestFixture(output).Clustered()) {}
      
    }
}