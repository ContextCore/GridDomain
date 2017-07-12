using Akka.Actor;
using GridDomain.Node.Actors.CommandPipe.Messages;

namespace GridDomain.Node.Actors.CommandPipe.Processors {
    public class SyncSagaProcessor: SynchronousMessageProcessor<ISagaTransitCompleted>
    {
        public SyncSagaProcessor(IActorRef processor) : base(processor) { }
    }
}