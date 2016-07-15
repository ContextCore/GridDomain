using System;
using GridDomain.EventSourcing.Sagas;
using GridDomain.Tests.Sagas.StateSagas.SubscriptionRenewSaga;
using GridDomain.Tests.Sagas.StateSagas.SubscriptionRenewSaga.Events;

namespace GridDomain.Tests.Sagas.StateSagas
{
    class SubscriptionRenewSagaFactory : 
        ISagaFactory<SubscriptionRenewSaga.SubscriptionRenewSaga, SubscriptionRenewSagaState>,
        ISagaFactory<SubscriptionRenewSaga.SubscriptionRenewSaga, SubscriptionExpiredEvent>,
        IEmptySagaFactory<SubscriptionRenewSaga.SubscriptionRenewSaga>
    {
        public SubscriptionRenewSaga.SubscriptionRenewSaga Create(SubscriptionRenewSagaState message)
        {
            return new SubscriptionRenewSaga.SubscriptionRenewSaga(message);
        }

        public SubscriptionRenewSaga.SubscriptionRenewSaga Create(SubscriptionExpiredEvent message)
        {
            return new SubscriptionRenewSaga.SubscriptionRenewSaga(new SubscriptionRenewSagaState(message.SagaId,
                SubscriptionRenewSaga.SubscriptionRenewSaga.States.SubscriptionSet));
        }

        public SubscriptionRenewSaga.SubscriptionRenewSaga Create()
        {
            return new SubscriptionRenewSaga.SubscriptionRenewSaga(new SubscriptionRenewSagaState(Guid.Empty,SubscriptionRenewSaga.SubscriptionRenewSaga.States.SubscriptionSet));
        }
    }
}