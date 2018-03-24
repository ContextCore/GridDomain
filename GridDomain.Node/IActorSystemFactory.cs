using Akka.Actor;

namespace GridDomain.Node
{
    public interface IActorSystemFactory
    {
        ActorSystem CreateSystem();
    }

    public interface IActorCommandPipeFactory
    {
        IActorCommandPipe CreatePipe();
    }
}