using Akka.Actor;
using GridDomain.Node.Actors.CommandPipe.Messages;

namespace GridDomain.Node.Actors.CommandPipe.MessageProcessors {
    public class SyncProcessManagerProcessor: ActorAskMessageProcessor<IProcessCompleted>
    {
        public SyncProcessManagerProcessor(IActorRef processor) : base(processor) { }
    }
}