using System;
using System.Threading.Tasks;
using Akka.Actor;
using GridDomain.Node;
using GridDomain.Node.Cluster;
using GridDomain.Node.Cluster.Transport;
using GridDomain.Node.Configuration;
using Serilog;

namespace GridDomain.Tests.Unit.Cluster {
    public static class NodeTestFixtureExtensions
    {
        private static GridDomainNode BuildClusterNode(this NodeTestFixture fxt, Func<ActorSystem> systemFactory, ILogger log)
        {
            var node = new GridNodeBuilder().PipeFactory(new DelegateActorSystemFactory(systemFactory,
                                                                                        sys =>
                                                                                        {
                                                                                            sys.AttachSerilogLogging(log);
                                                                                            sys.InitDistributedTransport();
                                                                                        }))
                                            .DomainConfigurations(fxt.DomainConfigurations.ToArray())
                                            .Log(log)
                                            .Timeout(fxt.DefaultTimeout)
                                            .BuildCluster();
            return (GridDomainNode)node;
        }

        public static NodeTestFixture Clustered(this NodeTestFixture fxt)
        {
            fxt.ConfigBuilder = c => c.ToClusterConfig();
            fxt.NodeBuilder = fxt.BuildClusterNode;
            
            return fxt;
        }
    }
}