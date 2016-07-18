using System;
using Automatonymous;

namespace GridDomain.EventSourcing.Sagas.InstanceSagas
{
    public class SagaMessageReceivedEvent<TSagaData> : SagaStateEvent
    {
        public object Message { get; }
        public TSagaData SagaData { get; }
        public Event MachineEvent { get; }

        public SagaMessageReceivedEvent(Guid sourceId, TSagaData sagaData, Event machineEvent, object externalMessage): base(sourceId)
        {
            SagaData = sagaData;
            MachineEvent = machineEvent;
            Message = externalMessage;
        }
    }
}