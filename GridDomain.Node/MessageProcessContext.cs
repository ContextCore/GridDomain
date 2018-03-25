using GridDomain.Common;
using GridDomain.Configuration;
using GridDomain.Configuration.MessageRouting;
using GridDomain.CQRS;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Transport;
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