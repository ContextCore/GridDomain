using System;
using GridDomain.Tests.Sagas.SubscriptionRenewSaga.Events;
using NUnit.Framework;

namespace GridDomain.Tests.Sagas.Simplified.Transitions
{
    [TestFixture]
    public class Given_AutomatonymousSaga_When_hydrating_state
    {
        private static void When_execute_valid_transaction(SagaInstance<SubscriptionRenewSagaData> sagaInstance,
            SubscriptionPaidEvent e = null)
        {
            var subscriptionPaidEvent = new SubscriptionPaidEvent();
            sagaInstance.Transit(e ?? subscriptionPaidEvent);
        }

        [Test]
        public void State_is_taken_from_events()
        {
            throw new NotImplementedException();
        }


        [Test]
        public void Saga_can_work_from_current_state_futher()
        {
            throw new NotImplementedException();
        }

        [Test]
        public void Saga_data_is_restored()
        {
            throw new NotImplementedException();
        }

        [Test]
        public void Saga_data_is_taken_from_event()
        {
            throw new NotImplementedException();
        }

    }
}