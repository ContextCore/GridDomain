using System;

namespace GridDomain.EventSourcing.Sagas
{
    public class SagaReceivedMessage<TSagaData> : SagaStateEvent
    {
        public SagaReceivedMessage(Guid sourceId, TSagaData state, object message)
            : base(sourceId)
        {
            State = state;
            Message = message;
        }

        //TODO: store type + message id to lower storage consumption
        public object Message { get; }
        public TSagaData State { get; }
    }
}