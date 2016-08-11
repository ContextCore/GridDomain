using System.Linq;
using GridDomain.Tests.Framework.Configuration;
using NUnit.Framework;

namespace GridDomain.Tests.Acceptance.MessageRoutingTests.GridNode.Cluster.Setup
{
    [TestFixture]
    internal abstract class ClusterActorSystemTest : ActorSystemTest<ClusterMessage, ClusterActorSystemInfrastructure>
    {
        protected override ClusterActorSystemInfrastructure CreateInfrastructure()
        {
            return new ClusterActorSystemInfrastructure(new AutoTestAkkaConfiguration());
        }

        protected override IGivenMessages<ClusterMessage> GivenCommands()
        {
            return new GivenClusterTestMessages(100);
        }

        [Test]
        public void Messages_should_be_processed_by_remote_nodes()
        {
            var nodeAddresses = Infrastructure.Nodes.Select(n => Akka.Cluster.Cluster.Get(n).SelfAddress).ToArray();
            var messageProcessedAddresses = _resultMessages.Select(m => m.ProcessorActorSystemAdress).Distinct();

            //messages should be processed by subset of all cluster nodes
            CollectionAssert.IsSubsetOf(messageProcessedAddresses, nodeAddresses);
        }

        [Test]
        public void Messages_should_be_processed_by_several_nodes()
        {
            var messageProcessedAddresses = _resultMessages.Select(m => m.ProcessorActorSystemAdress);
            Assert.Greater(messageProcessedAddresses.Count(), 1);
        }
    }
}