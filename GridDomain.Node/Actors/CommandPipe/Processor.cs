using Akka;
using Akka.Actor;

namespace GridDomain.Node.Actors
{
    public class Processor
    {
        public Processor(IActorRef actorRef, MessageProcessPolicy policy)
        {
            Policy = policy;
            ActorRef = actorRef;
        }

        public MessageProcessPolicy Policy { get; }
        public IActorRef ActorRef { get; }
    }

    //One per executing command
}