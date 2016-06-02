using Akka.Actor;
using GridDomain.Tests.Acceptance.Balance.MessageRoutingTests.GridNode.SingleSystem.Setup;

namespace GridDomain.Tests.Acceptance.Balance.MessageRoutingTests.GridNode.Cluster.Setup
{
    internal class ClusterMessage : TestMessage
    {
        public Address ProcessorActorSystemAdress { get; set; }
    }
}