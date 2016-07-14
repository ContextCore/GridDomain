using System;
using Automatonymous;

namespace GridDomain.Tests.Sagas.Simplified
{
    class SubscriptionRenewSagaInstance: SagaProgressAggregate<State>
    {
        public Guid BusinessId { get; private set; }
        public Guid SubscriptionId { get; private set; }
        public State CurrentState { get; private set; }

        private void ConfigureEventsApply()
        {
            
        }

        private SubscriptionRenewSagaInstance(Guid id) : base(id)
        {
            ConfigureEventsApply();
        }

        public SubscriptionRenewSagaInstance(Guid id, State state) : base(id,state)
        {
            ConfigureEventsApply();
        }


    }
}