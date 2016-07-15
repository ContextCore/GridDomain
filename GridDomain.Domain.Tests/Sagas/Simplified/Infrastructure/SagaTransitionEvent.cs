using System;
using Automatonymous;
using GridDomain.EventSourcing.Sagas;

namespace GridDomain.Tests.Sagas.Simplified
{
    public class SagaTransitionEvent<TSagaData> : SagaStateEvent
    {
        public TSagaData SagaData { get; }
        public State NewMachineState { get; set; }

        public SagaTransitionEvent(Guid sourceId, TSagaData sagaData, State newMachineState)
            : base(sourceId)
        {
            SagaData = sagaData;
            NewMachineState = newMachineState;
        }
    }
}