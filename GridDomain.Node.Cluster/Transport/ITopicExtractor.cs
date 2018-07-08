using System;

namespace GridDomain.Node.Cluster.Transport {
    public interface ITopicExtractor
    {
        string GetPublishTopic(object message);
        string GetSubscribeTopic(Type topic);
    }
}