using System;
using Automatonymous;
using GridDomain.EventSourcing.Sagas;

namespace GridDomain.Tests.Sagas.Simplified
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