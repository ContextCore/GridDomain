using Akka.Actor;
using GridDomain.Common;
using GridDomain.EventSourcing;
using GridDomain.Node.Actors.CommandPipe.Messages;

namespace GridDomain.Node.Cluster.CommandPipe {
    internal class DummyProcessActor : ReceiveActor
    {
        public DummyProcessActor()
        {
            Receive<IMessageMetadataEnvelop<DomainEvent>>(
                                                          m => Sender.Tell(new ProcessTransited(null, null, null, null)));
        }
    }
}