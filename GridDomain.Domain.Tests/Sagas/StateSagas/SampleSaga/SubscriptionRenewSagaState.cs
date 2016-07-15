using System;
using GridDomain.EventSourcing.Sagas;

namespace GridDomain.Tests.Sagas.StateSagas.SampleSaga
{
    public class SubscriptionRenewSagaState :
        SagaStateAggregate<SubscriptionRenewSaga.States, SubscriptionRenewSaga.Triggers>
    {
        public SubscriptionRenewSagaState(Guid id) : base(id)
        {
        }

        public SubscriptionRenewSagaState(Guid id, SubscriptionRenewSaga.States state) : base(id, state)
        {
        }
    }
}