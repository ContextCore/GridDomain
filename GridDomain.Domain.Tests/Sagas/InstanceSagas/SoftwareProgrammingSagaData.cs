using System;
using Automatonymous;
using GridDomain.EventSourcing.Sagas.InstanceSagas;

namespace GridDomain.Tests.Sagas.InstanceSagas
{
    class SoftwareProgrammingSagaData: ISagaState<State>
    {
        public Guid BusinessId { get; set; }
        public Guid SubscriptionId { get; set; }
        public State CurrentState { get; set; }

        public SoftwareProgrammingSagaData(State currentState)
        {
            CurrentState = currentState;
        }
    }
}