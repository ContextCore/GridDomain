using System;
using GridDomain.EventSourcing.Sagas;

namespace GridDomain.Tests.Sagas.Simplified
{
    public class SagaTransitionEvent<TSagaData> : SagaStateEvent
    {
        public object Message { get; }

        public TSagaData NewState { get; }

        public SagaTransitionEvent(Guid sourceId, TSagaData newState, object message)
            : base(sourceId)
        {
            NewState = newState;
            Message = message;
        }
    }
}