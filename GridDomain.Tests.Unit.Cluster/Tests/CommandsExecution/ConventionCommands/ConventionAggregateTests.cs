using GridDomain.Node;
using GridDomain.Node.Cluster;
using GridDomain.Tests.Unit.CommandsExecution.ConventionCommands;
using GridDomain.Tests.Unit.ProcessManagers.SoftwareProgrammingDomain;
using Serilog;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.Cluster.Tests.CommandsExecution.ConventionCommands
{

    public class ClusterConventionAggregateTests:ConventionAggregateTests
    {
        public ClusterConventionAggregateTests(ITestOutputHelper output) : base(new ClusterNodeTestFixture(output,new SoftwareDomainConfiguration())) { }

        protected override GridDomainNode CreateNode(NodeTestFixture fixture, ILogger logger)
        {
            Sys.InitDistributedTransport();
            return (GridDomainNode)fixture.CreateClusterNode(() => Sys,logger).Result;
        }
    }
}
