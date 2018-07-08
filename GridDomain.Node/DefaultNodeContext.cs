using Akka.Actor;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.Transport;
using Serilog;

namespace GridDomain.Node {
    public class DefaultNodeContext : INodeContext
    {
        public ICommandExecutor Executor { get; set; }
        public ActorSystem System { get;  set;}
        public IActorTransport Transport { get;  set;}
        public IPublisher Publisher => Transport;
        public ILogger Log { get;  set;}
    }
}