using System;
using System.Threading.Tasks;
using Akka.Actor;
using GridDomain.Node;
using GridDomain.Node.Cluster;
using GridDomain.Node.Configuration;
using Serilog;

namespace GridDomain.Tests.Unit.Cluster.Tests
{
    public static class NodeTestFixtureExtensions
    {
        public static async Task<IGridDomainNode> CreateClusterNode(this NodeTestFixture fxt, Func<ActorSystem> systemFactory, ILogger log)
        {
            var node = new GridNodeBuilder().PipeFactory(new DelegateActorSystemFactory(systemFactory))
                                            .DomainConfigurations(fxt.DomainConfigurations.ToArray())
                                            .Log(log)
                                            .Timeout(fxt.DefaultTimeout)
                                            .BuildCluster();

            return await fxt.StartNode((GridDomainNode) node);
        }
    }
}