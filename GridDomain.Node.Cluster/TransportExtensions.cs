using System;
using System.Linq;
using Akka.Actor;
using Akka.Persistence;
using GreenPipes.Internals.Extensions;
using GridDomain.Common;
using GridDomain.CQRS;
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
        public string GetPublishTopic(object message)
        {
            switch (message) {
                case IMessageMetadataEnvelop env:
                    return GetPublishTopic(env.Message);
                   
                case IFault f:
                    return "Fault_" + GetPublishTopic(f.Message);
            }

            return message.GetType().BeautyName();
        }

        public string GetSubscribeTopic(Type topic)
        {
            if (typeof(IFault).IsAssignableFrom(topic))
            {
                if (topic.ContainsGenericParameters && topic.GenericTypeArguments.Length == 1)
                    return "Fault_" + GetSubscribeTopic(topic.GenericTypeArguments.First());
                
                throw new NotSupportedException("Distributed pub-sub can support only Generic Faults for subscription");       
            }
                
            return topic.BeautyName();
        }
    }
}