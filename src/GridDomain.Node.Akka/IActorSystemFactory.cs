using Akka.Actor;

namespace GridDomain.Node.Akka
{
    public interface IActorSystemFactory
    {
        ActorSystem CreateSystem();
    }
}