using System;

namespace GridDomain.Node.AkkaMessaging.Waiting
{
    public class NotifyOnWaitEnd
    {
        public static readonly NotifyOnWaitEnd Instance = new NotifyOnWaitEnd();
    }

    public class NotifyOnMessage
    {
        public Type MessageType { get; }

        public NotifyOnMessage(Type messageType)
        {
            MessageType = messageType;
        }
    }
}