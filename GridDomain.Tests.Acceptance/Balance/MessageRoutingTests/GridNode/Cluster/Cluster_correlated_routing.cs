using System.Linq;
using GridDomain.Tests.Acceptance.MessageRoutingTests.GridNode.Cluster.Setup;
using GridDomain.Tests.Acceptance.MessageRoutingTests.GridNode.SingleSystem.Setup;
using NUnit.Framework;

namespace GridDomain.Tests.Acceptance.MessageRoutingTests.GridNode.Cluster
{
    [TestFixture]
    class Cluster_correlated_routing: ClusterActorSystemTest
    {
 
        [Test]
        public void Messages_should_be_processed_by_remote_nodes()
        {
            foreach(var node in Infrastructure.Nodes)
              Assert.True(_resultMessages.Any( m => m.ProcessorActorSystemAdress == Akka.Cluster.Cluster.Get(node).SelfAddress));
        }

        protected override IRouterConfiguration CreateRoutes()
        {
            return new CorrelatedRouting<ClusterMessage,TestHandler>(nameof(ClusterMessage.CorrelationId));
        }
    }
}