using GridDomain.Node.AkkaMessaging.Waiting;

namespace GridDomain.Node.Cluster.MessageWaiting {
    public class ClusterCorrelationConditionFactory<T> : CorrelationConditionFactory
    {
        public ClusterCorrelationConditionFactory(string correlationId) : base(correlationId) { }
    }
}