using System.Linq;
using Akka.Actor;
using Akka.Cluster;
using GridDomain.Node.AkkaMessaging;
using NUnit.Framework;

namespace GridDomain.Tests.Acceptance.MessageRoutingTests
{
    class Cluster_routing_correlated:Given_correlated_routing
    {
        private ActorSystem[] _clusterSystems;

    

        [TearDown]
        public void Clear()
        {
           
        }

 
        [Test]
        public void Messages_should_be_processed_by_remote_nodes()
        {
            Assert.True(_resultMessages.All( m => ((ClusterMessage)m).ProcessorActorSystemAdress != Cluster.Get(Infrastructure.System).SelfAddress));
        }
    }
}