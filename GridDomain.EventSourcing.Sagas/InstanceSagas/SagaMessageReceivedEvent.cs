using System;
using Automatonymous;

namespace GridDomain.EventSourcing.Sagas.InstanceSagas
{
    public class SagaMessageReceivedEvent<TSagaData> : SagaStateEvent
    {
        public object Message { get; }
        public TSagaData SagaData { get; }
        public string MachineEventName { get; }

        public SagaMessageReceivedEvent(Guid sourceId, TSagaData sagaData, Event machineEventName, object externalMessage): base(sourceId)
        {
            SagaData = sagaData;
            MachineEventName = machineEventName.Name;
            Message = externalMessage;
        }
    }
}