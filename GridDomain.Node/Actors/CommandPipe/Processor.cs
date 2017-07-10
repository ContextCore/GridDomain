using Akka.Actor;
using GridDomain.Node.Actors.CommandPipe.ProcessorCatalogs;

namespace GridDomain.Node.Actors.CommandPipe
{
    public class Processor
    {
        public Processor(IActorRef actorRef, MessageProcessPolicy policy = null)
        {
            Policy = policy ?? new MessageProcessPolicy(false);
            ActorRef = actorRef;
        }

        public MessageProcessPolicy Policy { get; }
        public IActorRef ActorRef { get; }
    }
}