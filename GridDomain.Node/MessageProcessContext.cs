using GridDomain.CQRS.Messaging;
using GridDomain.CQRS.Messaging.Akka;
using GridDomain.Node.Configuration.Composition;

namespace GridDomain.Node {
    public class MessageProcessContext : IMessageProcessContext
    {
        public MessageProcessContext(IActorTransport transport)
        {
            Publisher = transport;
        }

        public IPublisher Publisher { get; }
    }
}