using System;
using Automatonymous;

namespace GridDomain.EventSourcing.Sagas.InstanceSagas
{
    public class SagaTransitionEvent<TSagaData> : SagaStateEvent
    {
        public TSagaData SagaData { get; }

        public SagaTransitionEvent(Guid sourceId, TSagaData sagaData)
            : base(sourceId)
        {
            SagaData = sagaData;
        }
    }

    public class InstanceSagaTransitionEvent<TSagaData> : SagaStateEvent
    {
        public TSagaData SagaData { get; }
        public string StateName { get; set; }

        public InstanceSagaTransitionEvent(Guid sourceId, TSagaData sagaData, string stateName)
            : base(sourceId)
        {
            SagaData = sagaData;
            StateName = stateName;
        }

        public InstanceSagaTransitionEvent<TSagaData> New(Guid sourceId, TSagaData sagaData, State newMachineState)
        {
            return new InstanceSagaTransitionEvent<TSagaData>(sourceId, sagaData, newMachineState.Name);
        }
    }
}