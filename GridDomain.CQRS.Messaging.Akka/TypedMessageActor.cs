using Akka.Actor;

namespace GridDomain.CQRS.Messaging.Akka
{
    public class TypedMessageActor<T> : IHandler<T>
    {
        public readonly IActorRef Actor;

        public TypedMessageActor(IActorRef actor)
        {
            Actor = actor;
        }

        public void Handle(T msg)
        {
            Actor.Tell(msg);
        }
    }
}