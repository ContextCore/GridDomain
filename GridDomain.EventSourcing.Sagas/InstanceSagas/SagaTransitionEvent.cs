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
}