using Akka.Actor;

namespace GridDomain.Transport.Extension {
    public class TransportExtension : IExtension
    {
        public IActorTransport Transport { get; }

        public TransportExtension(IActorTransport transport)
        {
            Transport = transport;
        }
    }
}