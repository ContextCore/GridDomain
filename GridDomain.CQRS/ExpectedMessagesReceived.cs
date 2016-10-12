namespace GridDomain.CQRS
{
    public class ExpectedMessagesReceived<T> : ExpectedMessagesReceived
    {
        public ExpectedMessagesReceived(T msg, params object[] objects) : base(msg, objects)
        {
            Message = msg;
        }

        public new T Message { get;}
    }
}