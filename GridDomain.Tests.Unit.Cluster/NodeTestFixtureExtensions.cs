using System;
using System.Threading.Tasks;
using GridDomain.Node;
using GridDomain.Node.Cluster;
using GridDomain.Node.Configuration;
using Serilog;

namespace GridDomain.Tests.Unit.Cluster {
    public static class NodeTestFixtureExtensions
    {
        public static async Task<IGridDomainNode> CreateClusterNode(this NodeTestFixture fxt, Func<Akka.Cluster.Cluster> clusterProducer, ILogger log)
        {
            var node = new GridNodeBuilder().PipeFactory(new DelegateActorSystemFactory(() => clusterProducer()
                                                                                            .System))
                                            .DomainConfigurations(fxt.DomainConfigurations.ToArray())
                                            .Log(log)
                                            .Timeout(fxt.DefaultTimeout)
                                            .BuildCluster();

            return await fxt.StartNode((GridDomainNode) node);
        }
    }
}