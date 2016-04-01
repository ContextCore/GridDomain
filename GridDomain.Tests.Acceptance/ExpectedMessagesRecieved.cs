using System;

namespace GridDomain.Tests.Acceptance
{
    public class ExpectedMessagesRecieved<T>
    {
        public ExpectedMessagesRecieved(T msg, int numLeft, Guid[] sources)
        {
            NumLeft = numLeft;
            Sources = sources;
            Message = msg;
        }

        public int NumLeft { get; set; }
        public Guid[] Sources { get; set; }

        public T Message { get; set; }
    }
}