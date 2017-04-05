using System;
using GridDomain.EventSourcing.Sagas.InstanceSagas;

namespace GridDomain.Node.Actors
{
    internal class SagaAlreadyStartedException : Exception
    {
        public object StartMessage { get; }
        public ISagaState ExistingState { get; }

        public SagaAlreadyStartedException(ISagaState existingState, object startMessage)
        {
            StartMessage = startMessage;
            ExistingState = existingState;
        }
    }
}