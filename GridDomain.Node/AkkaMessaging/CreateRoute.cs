using System;

namespace GridDomain.Node.AkkaMessaging
{
    public class CreateRoute : IEquatable<CreateRoute>
    {
        public Type HandlerType;
        public string MessageCorrelationPropertyName;
        public Type MessageType;

        public bool Equals(CreateRoute other)
        {
            return other.HandlerType == HandlerType && other.MessageType == MessageType;
        }
    }
}