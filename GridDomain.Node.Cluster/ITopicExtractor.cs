using System;

namespace GridDomain.Node.Cluster {
    public interface ITopicExtractor
    {
        string GetPublishTopic(object message);
        string GetSubscribeTopic(Type topic);
    }
}