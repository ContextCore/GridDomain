using System.Linq;
using Akka.Cluster;
using NUnit.Framework;

namespace GridDomain.Tests.Acceptance.MessageRoutingTests
{
    [TestFixture]
    class Cluster_not_correlated_routing : ClusterActorSystemTest
    {

        [Test]
        public void Messages_should_be_processed_by_remote_nodes()
        {
            foreach (var node in Infrastructure.Nodes)
                Assert.True(_resultMessages.Any(m => m.ProcessorActorSystemAdress == Cluster.Get(node).SelfAddress));
        }

        protected override IRouterConfiguration CreateRoutes()
        {
            return new NotCorrelatedRouting<ClusterMessage, TestHandler>();
        }
    }
}