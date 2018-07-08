using Akka.Actor;
using GridDomain.Configuration;
using GridDomain.CQRS;
using GridDomain.Transport;

namespace GridDomain.Node {
    public interface INodeContext :IMessageProcessContext
    {
        ActorSystem System { get;}
        IActorTransport Transport { get; }
        ICommandExecutor Executor { get; }
    }
}