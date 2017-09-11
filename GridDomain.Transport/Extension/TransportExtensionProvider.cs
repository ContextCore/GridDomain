using Akka.Actor;

namespace GridDomain.Transport.Extension {
    public class TransportExtensionProvider : ExtensionIdProvider<TransportExtension>
    {
        private readonly IActorTransport _transport;

        public TransportExtensionProvider(IActorTransport transport)
        {
            _transport = transport;
        }
        public override TransportExtension CreateExtension(ExtendedActorSystem system)
        {
            return new TransportExtension(_transport);
        }
    }
}