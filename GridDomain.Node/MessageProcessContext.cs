using GridDomain.CQRS.Messaging;
using GridDomain.CQRS.Messaging.Akka;
using GridDomain.Node.Configuration.Composition;
using Serilog;

namespace GridDomain.Node {
    public class MessageProcessContext : IMessageProcessContext
    {
        public MessageProcessContext(IActorTransport transport, ILogger log)
        {
            Publisher = transport;
            Log = log;
        }

        public IPublisher Publisher { get; }
        public ILogger Log { get; }
    }
}