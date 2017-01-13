using GridDomain.Common;
using GridDomain.CQRS.Messaging;
using GridDomain.Node.Actors;
using GridDomain.Tests.Acceptance.MessageRoutingTests.GridNode.SingleSystem.Setup;

namespace GridDomain.Tests.Acceptance.MessageRoutingTests.GridNode.Cluster.Setup
{
    internal class ClusterMessageHandlerActor : MessageHandlingActor<ClusterMessage, TestHandler>
    {
        public ClusterMessageHandlerActor(TestHandler handler, IPublisher publisher) : base(handler, publisher)
        {
        }
    }
}