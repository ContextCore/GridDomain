using GridDomain.Node.Configuration;

namespace GridDomain.Tests.Acceptance.MessageRoutingTests.GridNode.Cluster.Setup
{
    abstract class ClusterActorSystemTest : ActorSystemTest<ClusterMessage, ClusterActorSystemInfrastructure>
    {
        protected override ClusterActorSystemInfrastructure CreateInfrastructure()
        {
            return new ClusterActorSystemInfrastructure(new AkkaConfiguration("testCluster", 8100, "127.0.0.1"));
        }

        protected override IGivenCommands<ClusterMessage> GivenCommands()
        {
            return new GivenClusterTestMessages();
        }
    }
}