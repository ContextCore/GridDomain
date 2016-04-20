using Akka.Cluster;
using GridDomain.Node.AkkaMessaging;

namespace GridDomain.Tests.Acceptance.MessageRoutingTests
{
    class ClusterMessageHandlerActor : MessageHandlingActor<ClusterMessage, TestHandler>
    {
        public ClusterMessageHandlerActor(TestHandler handler) : base(handler)
        {
        }

        protected override void OnReceive(object msg)
        {
            ((ClusterMessage)msg).ProcessorActorSystemAdress = Cluster.Get(Context.System).SelfAddress;
            base.OnReceive(msg);
        }
    }
}