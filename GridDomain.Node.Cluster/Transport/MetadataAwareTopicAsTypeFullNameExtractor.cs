using System;
using System.Linq;
using GridDomain.Common;
using GridDomain.CQRS;

namespace GridDomain.Node.Cluster.Transport {
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
                if (topic.IsConstructedGenericType && topic.GenericTypeArguments.Length == 1)
                    return "Fault_" + GetSubscribeTopic(topic.GenericTypeArguments.First());
                
                throw new NotSupportedException("Distributed pub-sub can support only Generic Faults for subscription. Topic subscribed:" + topic.FullName);       
            }
                
            return topic.BeautyName();
        }
    }
}