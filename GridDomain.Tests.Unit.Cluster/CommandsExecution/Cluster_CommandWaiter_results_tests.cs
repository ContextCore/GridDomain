using GridDomain.Tests.Unit.CommandsExecution;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.Cluster.CommandsExecution
{
    public class Cluster_CommandWaiter_results_tests : CommandWaiter_results_tests
    {
        public Cluster_CommandWaiter_results_tests(ITestOutputHelper output)
            : base(new NodeTestFixture(output).Clustered()) {}
    }
}