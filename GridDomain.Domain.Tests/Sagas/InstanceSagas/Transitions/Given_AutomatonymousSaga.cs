using System;
using Automatonymous;
using GridDomain.EventSourcing.Sagas.InstanceSagas;

namespace GridDomain.Tests.Sagas.InstanceSagas.Transitions
{
    class Given_AutomatonymousSaga
    {
        public readonly SubscriptionRenewSaga SagaMachine = new SubscriptionRenewSaga();
        public readonly SagaInstance<SubscriptionRenewSagaData> SagaInstance;
        public readonly SagaDataAggregate<SubscriptionRenewSagaData> SagaDataAggregate;

        public Given_AutomatonymousSaga(Func<SubscriptionRenewSaga, State> initialState)
        {
            var sagaData = new SubscriptionRenewSagaData(initialState(SagaMachine));
            SagaDataAggregate = new SagaDataAggregate<SubscriptionRenewSagaData>(Guid.NewGuid(), sagaData);
            SagaInstance = new SagaInstance<SubscriptionRenewSagaData>(SagaMachine, SagaDataAggregate);
        }
    }
}