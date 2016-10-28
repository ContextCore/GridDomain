using System.Collections.Generic;

namespace GridDomain.CQRS
{
    public class ExpectedMessagesReceived
    {
        public ExpectedMessagesReceived(object msg, params object[] objects)
        {
            Message = msg;
            Received = objects;
        }

        public object Message { get; private set; }

        public IReadOnlyCollection<object> Received { get; }
    }

    public class ExpectedMessagesReceived<T> : ExpectedMessagesReceived
    {
        public ExpectedMessagesReceived(T msg, params object[] objects) : base(msg, objects)
        {
            Message = msg;
        }

        public new T Message { get;}
    }
}