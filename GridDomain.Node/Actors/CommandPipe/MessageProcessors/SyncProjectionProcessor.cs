using Akka.Actor;
using GridDomain.Node.Actors.Hadlers;

namespace GridDomain.Node.Actors.CommandPipe.MessageProcessors {
    public class SyncProjectionProcessor : SynchronousMessageProcessor<HandlerExecuted>
    {
        public SyncProjectionProcessor(IActorRef processor) : base(processor) { }
    }
}