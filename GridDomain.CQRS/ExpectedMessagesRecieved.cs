using System.Collections.Generic;

namespace GridDomain.CQRS
{
    public class ExpectedMessagesReceived<T> : ExpectedMessagesReceived
    {
        public ExpectedMessagesReceived(T msg) : base(msg, new object[] {})
        {
            Message = msg;
        }

        public new T Message { get;}
    }

    public class ExpectedMessagesReceived
    {
        public ExpectedMessagesReceived(object msg, object[] objects)
        {
            Message = msg;
            Recieved = objects;
        }

        public object Message { get; private set; }

        public IReadOnlyCollection<object> Recieved { get; } 
    }
}