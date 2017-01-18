using Akka.Actor;
using GridDomain.Tests.Acceptance.MessageRoutingTests.GridNode.SingleSystem.Setup;

namespace GridDomain.Tests.Acceptance.MessageRoutingTests.GridNode.Cluster.Setup
{
    internal class ClusterEvent : TestEvent
    {
        public Address ProcessorActorSystemAdress { get; set; }
    }
}