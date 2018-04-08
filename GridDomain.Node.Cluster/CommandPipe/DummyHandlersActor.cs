using Akka.Actor;
using GridDomain.Common;
using GridDomain.Node.Actors.CommandPipe.Messages;

namespace GridDomain.Node.Cluster.CommandPipe {
    public class DummyHandlersActor : ReceiveActor
    {
        public DummyHandlersActor()
        {
            Receive<IMessageMetadataEnvelop>(envelop => Sender.Tell(AllHandlersCompleted.Instance));
            Receive<ProcessesTransitComplete>(t => { });
        }
    }
}