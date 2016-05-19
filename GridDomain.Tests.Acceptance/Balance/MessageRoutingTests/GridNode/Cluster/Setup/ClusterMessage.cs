using Akka.Actor;
using GridDomain.Tests.Acceptance.MessageRoutingTests.GridNode.SingleSystem.Setup;

namespace GridDomain.Tests.Acceptance.MessageRoutingTests.GridNode.Cluster.Setup
{
    class ClusterMessage : TestMessage
    {
        public Address ProcessorActorSystemAdress { get; set; }
    }
}
