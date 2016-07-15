using Automatonymous;

namespace GridDomain.EventSourcing.Sagas.InstanceSagas
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


    public class EventReceivedData<TEventData, TSagaData> : EventReceivedData<TSagaData>
    {
        public EventReceivedData(Event @event, TEventData eventData, TSagaData sagaData) :base(@event,eventData,sagaData)
        {
        }
        public new TEventData EventData => (TEventData) base.EventData;
    }
}