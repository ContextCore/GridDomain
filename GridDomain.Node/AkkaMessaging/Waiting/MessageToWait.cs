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

        public static MessageToWait Once(Type messageType)
        {
            return new MessageToWait(messageType, 1);
        }

        public static MessageToWait Once<T>()
        {
            return new MessageToWait(typeof(T), 1);
        }
    }
}