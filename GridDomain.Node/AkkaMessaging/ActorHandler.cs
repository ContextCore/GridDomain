using Akka.Actor;
using GridDomain.CQRS;

namespace GridDomain.Node.AkkaMessaging
{
    public class ActorHandler<T>:IHandler<T>
    {
        private readonly IActorRef _actor;

        public ActorHandler(IActorRef actor)
        {
            _actor = actor;
        }

        public void Handle(T msg)
        {
            _actor.Tell(msg);
        }
    }
}