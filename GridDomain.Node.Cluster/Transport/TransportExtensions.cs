using Akka.Actor;
using GridDomain.Transport.Extension;

namespace GridDomain.Node.Cluster.Transport {
    public static class TransportExtensions
    {
        public static TransportExtension InitDistributedTransport(this ActorSystem system)
        {
            return  (TransportExtension)system.RegisterExtension(new TransportExtensionProvider(new DistributedPubSubTransport(system,new MetadataAwareTopicAsTypeFullNameExtractor())));
        }
    }
}