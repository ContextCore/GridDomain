using System.Collections.Generic;

namespace GridDomain.Node.AkkaMessaging.Waiting
{
    public class ExpectedMessagesRecieved<T> : ExpectedMessagesRecieved
    {
        public ExpectedMessagesRecieved(T msg) : base(msg, new List<object>())
        {
            Message = msg;
        }

        public new T Message { get;}
    }

    public class ExpectedMessagesRecieved
    {
        public ExpectedMessagesRecieved(object msg, List<object> objects)
        {
            Message = msg;
            Recieved = objects;
        }

        public object Message { get; private set; }

        public List<object> Recieved { get; } 
    }
}