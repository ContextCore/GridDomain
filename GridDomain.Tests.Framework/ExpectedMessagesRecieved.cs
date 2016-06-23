namespace GridDomain.Tests.Framework
{
    public class ExpectedMessagesRecieved<T> : ExpectedMessagesRecieved
    {
        public ExpectedMessagesRecieved(T msg) : base(msg)
        {
            Message = msg;
        }

        public new T Message { get;}
    }

    public class ExpectedMessagesRecieved
    {
        public ExpectedMessagesRecieved(object msg)
        {
            Message = msg;
        }

        public object Message { get; private set; }
    }
}