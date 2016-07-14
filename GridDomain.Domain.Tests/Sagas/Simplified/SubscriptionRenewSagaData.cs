using System;
using Automatonymous;
using Ploeh.AutoFixture;

namespace GridDomain.Tests.Sagas.Simplified
{
    class SubscriptionRenewSagaData: ISagaState<State>
    {
        public Guid BusinessId { get; set; }
        public Guid SubscriptionId { get; set; }
        public State CurrentState { get; set; }

        public SubscriptionRenewSagaData(State currentState)
        {
            CurrentState = currentState;
        }
    }
}