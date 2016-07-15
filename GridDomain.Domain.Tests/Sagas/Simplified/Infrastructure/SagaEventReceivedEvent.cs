using System;
using Automatonymous;
using GridDomain.EventSourcing.Sagas;

namespace GridDomain.Tests.Sagas.Simplified
{
    public class SagaMessageReceivedEvent<TSagaData> : SagaStateEvent
    {
        public object Message { get; }
        public TSagaData SagaDataBeforeEvent { get; }
        public Event MachineEvent { get; }

        public SagaMessageReceivedEvent(Guid sourceId, TSagaData sagaDataBeforeEvent, Event machineEvent, object externalMessage): base(sourceId)
        {
            SagaDataBeforeEvent = sagaDataBeforeEvent;
            MachineEvent = machineEvent;
            Message = externalMessage;
        }
    }
}