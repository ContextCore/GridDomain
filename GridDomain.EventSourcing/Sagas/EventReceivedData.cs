using Automatonymous;

namespace GridDomain.EventSourcing.Sagas
{
    public class EventReceivedData<TSagaData>
    {
        public EventReceivedData(Event @event, object eventData, TSagaData sagaData)
        {
            Event = @event;
            EventData = eventData;
            SagaData = sagaData;
        }

        public object EventData { get; }
        public TSagaData SagaData { get; }
        public Event Event { get; }
    }
}