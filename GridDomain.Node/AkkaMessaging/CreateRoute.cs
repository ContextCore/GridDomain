using System;

namespace GridDomain.Node.AkkaMessaging
{
    public class CreateRoute: IEquatable<CreateRoute>
    {
        public Type MessageType;
        public Type HandlerType;
        public string MessageCorrelationPropertyName;
        public bool Equals(CreateRoute other)
        {
            return other.HandlerType == HandlerType && other.MessageType == MessageType;
        }
    }
}