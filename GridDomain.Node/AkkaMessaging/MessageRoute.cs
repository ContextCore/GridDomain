using System;

namespace GridDomain.Node.AkkaMessaging
{
    public class MessageRoute
    {
        public Type MessageType { get; set; }
        public string CorrelationField { get; set; }
    }
}