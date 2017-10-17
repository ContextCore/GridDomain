using Akka.Actor;

namespace GridDomain.Node
{
    public interface IActorSystemFactory
    {
        ActorSystem Create();
    }
}