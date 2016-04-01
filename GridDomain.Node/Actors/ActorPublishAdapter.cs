using Akka.Actor;
using GridDomain.CQRS.Messaging;

namespace GridDomain.Node.Actors
{
    public class ActorPublishAdapter : IPublisher
    {
        private readonly IActorRef _actorRef;

        public ActorPublishAdapter(IActorRef actorRef)
        {
            _actorRef = actorRef;
        }

        public void Publish<T>(T msg)
        {
            _actorRef.Tell(msg);
        }
    }
}