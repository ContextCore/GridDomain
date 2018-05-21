using System;
using GridDomain.Node.AkkaMessaging.Waiting;

namespace GridDomain.Node.Cluster.MessageWaiting {
    public class ClusterCorrelationConditionBuilder<T> : ConditionBuilder
    {
        private readonly string _correlationId;

        public ClusterCorrelationConditionBuilder(string correlationId) => _correlationId = correlationId;

        protected override bool CheckMessageType(object receivedMessage, Type t, Func<object, bool> domainMessageFilter = null)
        {
            return base.CheckMessageType(receivedMessage.SafeUnenvelope(), t, domainMessageFilter);
        }

        protected override Func<object, bool> AddFilter(Type messageType, Func<object, bool> filter = null)
        {
            bool CorrelationFilter(object m) => m.SafeCheckCorrelation(_correlationId)
                                                && base.CheckMessageType(m, messageType, filter);

            base.AddFilter(messageType, CorrelationFilter);

            return CorrelationFilter;
        }
    }
}