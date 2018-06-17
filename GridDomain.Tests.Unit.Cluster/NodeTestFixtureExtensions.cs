using System;
using System.Threading.Tasks;
using Akka.Actor;
using GridDomain.Configuration;
using GridDomain.Node;
using GridDomain.Node.Cluster;
using GridDomain.Node.Cluster.Transport;
using GridDomain.Node.Configuration;
using Serilog;

namespace GridDomain.Tests.Unit.Cluster
{
    public static class NodeTestFixtureExtensions
    {
        public static NodeTestFixture Clustered(this NodeTestFixture fxt)
        {
            fxt.ActorSystemConfigBuilder.ConfigureCluster(fxt.Name);

            fxt.NodeBuilder = new ClusterNodeBuilder();

            var baseNodeSetup = fxt.NodeBuilderConfigurator;
            fxt.NodeBuilderConfigurator = (actorSystemProducer, log, builder) =>
                                          {
                                              baseNodeSetup(actorSystemProducer, log, builder);
                                              builder.Initialize(sys =>
                                                                 {
                                                                     sys.AttachSerilogLogging(log);
                                                                     sys.InitDistributedTransport();
                                                                 });
                                          };
            
            fxt.TestNodeBuilder = (n, kit) => new TestClusterNode((GridClusterNode) n, kit);

            return fxt;
        }
    }

    public class ClusterNodeBuilder : GridNodeBuilder
    {
        public override IGridDomainNode Build()
        {
            var factory = new DelegateActorSystemFactory(_actorProducers, _actorInit);

            return new GridClusterNode(Configurations, factory, Logger, DefaultTimeout);
        }
    }
}