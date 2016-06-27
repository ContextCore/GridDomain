using System;

namespace GridDomain.Node.AkkaMessaging.Waiting
{
    public class MessageToWait
    {
        public MessageToWait(Type messageType, int count)
        {
            MessageType = messageType;
            Count = count;
        }

        public Type MessageType { get; }
        public int Count { get; }
    }
}