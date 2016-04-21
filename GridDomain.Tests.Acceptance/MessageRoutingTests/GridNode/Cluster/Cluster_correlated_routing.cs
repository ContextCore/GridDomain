using System.Linq;
using Akka.Actor;
using Akka.Cluster;
using GridDomain.Node.AkkaMessaging;
using NUnit.Framework;

namespace GridDomain.Tests.Acceptance.MessageRoutingTests
{
    [TestFixture]
    class Cluster_correlated_routing: ClusterActorSystemTest
    {
 
        [Test]
        public void Messages_should_be_processed_by_remote_nodes()
        {
            foreach(var node in Infrastructure.Nodes)
              Assert.True(_resultMessages.Any( m => m.ProcessorActorSystemAdress == Cluster.Get(node).SelfAddress));
        }

        protected override IRouterConfiguration CreateRoutes()
        {
            return new CorrelatedRouting<ClusterMessage,TestHandler>(nameof(ClusterMessage.CorrelationId));
        }
    }
}