using GridDomain.Node.AkkaMessaging.Waiting;

namespace GridDomain.Node.Cluster {
    public class ClusterCorrelationConditionBuilder<T> : CorrelationConditionBuilder<T>
    {
        public ClusterCorrelationConditionBuilder(string correlationId) : base(correlationId) { }
    }
}