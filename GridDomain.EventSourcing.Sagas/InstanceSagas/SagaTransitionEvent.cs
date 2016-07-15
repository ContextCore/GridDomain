using System;
using Automatonymous;

namespace GridDomain.EventSourcing.Sagas.InstanceSagas
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