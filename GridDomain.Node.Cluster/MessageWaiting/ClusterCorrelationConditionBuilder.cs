using System;
using GridDomain.Node.AkkaMessaging.Waiting;

namespace GridDomain.Node.Cluster.MessageWaiting {
    public class ClusterCorrelationConditionBuilder : ConditionBuilder
    {
        private readonly string _correlationId;

        public ClusterCorrelationConditionBuilder(string correlationId) => _correlationId = correlationId;

        protected override bool CheckMessageType(object receivedMessage, Type t, Func<object, bool> domainMessageFilter = null)
        {
            return base.CheckMessageType(receivedMessage.SafeUnenvelope(), t, domainMessageFilter);
        }

        protected override Func<object, bool> AddFilter(Type messageType, Func<object, bool> filter = null)
        {
            AcceptedMessageTypes.Add(messageType);
            bool FilterWithAdapter(object o) => o.SafeCheckCorrelation(_correlationId) && CheckMessageType(o, messageType, filter);
            MessageFilters.Add(FilterWithAdapter);
            return FilterWithAdapter;
        }
    }
}