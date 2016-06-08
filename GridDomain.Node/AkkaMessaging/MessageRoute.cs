using System;

namespace GridDomain.Node.AkkaMessaging
{
    public class MessageRoute
    {
        public MessageRoute(Type messageType, string correlationField = null)
        {
            MessageType = messageType;
            CorrelationField = correlationField;
        }

        public Type MessageType { get; }
        public string CorrelationField { get; }
    }
}