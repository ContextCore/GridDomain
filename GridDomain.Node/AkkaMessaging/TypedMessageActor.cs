using Akka.Actor;
using GridDomain.CQRS;

namespace GridDomain.Node.AkkaMessaging
{
    public class TypedMessageActor<T> : IHandler<T>
    {
        public readonly IActorRef Actor;

        public TypedMessageActor(IActorRef actor)
        {
            Actor = actor;
        }

        public void Handle(T e)
        {
            Actor.Tell(e);
        }
    }
}