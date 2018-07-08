using GridDomain.Node;
using GridDomain.Node.Cluster;
using GridDomain.Tests.Unit.CommandsExecution.ConventionCommands;
using GridDomain.Tests.Unit.ProcessManagers.SoftwareProgrammingDomain;
using Serilog;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.Cluster.CommandsExecution.ConventionCommands
{

    public class ClusterConventionAggregateTests:ConventionAggregateTests
    {
        public ClusterConventionAggregateTests(ITestOutputHelper output) :
            base(new NodeTestFixture(output,new SoftwareDomainConfiguration()).Clustered().PrintSystemConfig()) { }
    }
}
