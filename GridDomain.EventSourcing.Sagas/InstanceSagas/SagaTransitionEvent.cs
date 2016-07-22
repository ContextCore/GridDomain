using System;
using Automatonymous;

namespace GridDomain.EventSourcing.Sagas.InstanceSagas
{
    public class SagaTransitionEvent<TSagaData> : SagaStateEvent
    {
        public TSagaData SagaData { get; }
        public string NewMachineState { get; set; }

        public SagaTransitionEvent(Guid sourceId, TSagaData sagaData, string newMachineState)
            : base(sourceId)
        {
            SagaData = sagaData;
            NewMachineState = newMachineState;
        }

        public SagaTransitionEvent<TSagaData> New(Guid sourceId, TSagaData sagaData, State newMachineState)
        {
            return new SagaTransitionEvent<TSagaData>(sourceId, sagaData, newMachineState.Name);
        }
    }
}