using System;

namespace GridDomain.Node.AkkaMessaging.Waiting
{
    public class NotifyOnMessage
    {
        public NotifyOnMessage(Type messageType)
        {
            MessageType = messageType;
        }

        public Type MessageType { get; }
    }
}