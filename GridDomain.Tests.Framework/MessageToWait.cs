using System;

namespace GridDomain.Tests.Framework
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