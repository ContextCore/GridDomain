using System;

namespace GridDomain.Node.AkkaMessaging.Waiting
{
    public class NotifyOnMessage
    {
        public Type MessageType { get; }

        public NotifyOnMessage(Type messageType)
        {
            MessageType = messageType;
        }
    }
}