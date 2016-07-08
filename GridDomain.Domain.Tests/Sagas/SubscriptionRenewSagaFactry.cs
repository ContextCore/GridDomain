using System;
using GridDomain.EventSourcing.Sagas;
using GridDomain.Tests.Sagas.SubscriptionRenew;
using GridDomain.Tests.Sagas.SubscriptionRenew.Events;

namespace GridDomain.Tests.Sagas
{
    class SubscriptionRenewSagaFactory : 
        ISagaFactory<SubscriptionRenewSaga, SubscriptionRenewSagaState>,
        ISagaFactory<SubscriptionRenewSaga, SubscriptionExpiredEvent>,
        IEmptySagaFactory<SubscriptionRenewSaga>
    {
        public SubscriptionRenewSaga Create(SubscriptionRenewSagaState message)
        {
            return new SubscriptionRenewSaga(message);
        }

        public SubscriptionRenewSaga Create(SubscriptionExpiredEvent message)
        {
            return new SubscriptionRenewSaga(new SubscriptionRenewSagaState(message.SagaId,
                SubscriptionRenewSaga.States.SubscriptionSet));
        }

        public SubscriptionRenewSaga Create()
        {
            return new SubscriptionRenewSaga(new SubscriptionRenewSagaState(Guid.Empty,SubscriptionRenewSaga.States.SubscriptionSet));
        }
    }
}