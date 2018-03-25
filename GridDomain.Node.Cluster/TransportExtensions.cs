using System;
using Akka.Actor;
using GridDomain.Common;
using GridDomain.Transport;
using GridDomain.Transport.Extension;

namespace GridDomain.Node.Cluster {
    public static class TransportExtensions
    {
        public static TransportExtension InitDistributedTransport(this ActorSystem system)
        {
            return  (TransportExtension)system.RegisterExtension(new TransportExtensionProvider(new DistributedPubSubTransport(system,new MetadataAwareTopicAsTypeFullNameExtractor())));
        }
    }

    public class MetadataAwareTopicAsTypeFullNameExtractor : ITopicExtractor
    {
        public string GetTopic(object message)
        {
            switch (message) {
                case IMessageMetadataEnvelop env:
                    return env.Message.GetType()
                              .FullName;
                case Type t:
                    return t.FullName;
            }

            return message.GetType().FullName;
        }
    }
}