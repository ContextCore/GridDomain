namespace GridDomain.EventSourcing.Sagas
{
    public class MessageReceivedData<TSagaData>
    {
        public MessageReceivedData(object message, TSagaData data)
        {
            Message = message;
            SagaData = data;
        }

        public object Message { get; }
        public TSagaData SagaData { get; }
    }
}