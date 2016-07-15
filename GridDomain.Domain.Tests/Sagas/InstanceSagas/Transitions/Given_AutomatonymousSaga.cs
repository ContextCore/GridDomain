using System;
using Automatonymous;
using GridDomain.EventSourcing.Sagas.InstanceSagas;

namespace GridDomain.Tests.Sagas.Simplified.Transitions
{
    class Given_AutomatonymousSaga
    {
        public readonly SubscriptionRenewSagaSimplified SagaMachine = new SubscriptionRenewSagaSimplified();
        public readonly SagaInstance<SubscriptionRenewSagaData> SagaInstance;
        public readonly SagaDataAggregate<SubscriptionRenewSagaData> SagaDataAggregate;

        public Given_AutomatonymousSaga(Func<SubscriptionRenewSagaSimplified, State> initialState)
        {
            var sagaData = new SubscriptionRenewSagaData(initialState(SagaMachine));
            SagaDataAggregate = new SagaDataAggregate<SubscriptionRenewSagaData>(Guid.NewGuid(), sagaData);
            SagaInstance = new SagaInstance<SubscriptionRenewSagaData>(SagaMachine, SagaDataAggregate);
        }
    }
}