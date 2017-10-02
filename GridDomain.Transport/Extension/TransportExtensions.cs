using Akka.Actor;

namespace GridDomain.Transport.Extension {
    public static class TransportExtensions
    {
        public static TransportExtension InitLocalTransportExtension(this ActorSystem system)
        {
            return  (TransportExtension)system.RegisterExtension(new TransportExtensionProvider(new LocalAkkaEventBusTransport(system)));
        }

        public static IActorTransport GetTransport(this ActorSystem system)
        {
            return system.GetExtension<TransportExtension>().Transport;
        }
    }
}