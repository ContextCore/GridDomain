using System;

namespace GridDomain.EventSourcing.Sagas.InstanceSagas
{
    public class SagaMessageReceivedEvent<TSagaData> : SagaStateEvent
    {
        public SagaMessageReceivedEvent(Guid sourceId, TSagaData sagaData, string machineEventName, object message)
            : base(sourceId)
        {
            SagaData = sagaData;
            MachineEventName = machineEventName;
            Message = message;
        }

        public object Message { get; }
        public TSagaData SagaData { get; }
        public string MachineEventName { get; }
    }
}