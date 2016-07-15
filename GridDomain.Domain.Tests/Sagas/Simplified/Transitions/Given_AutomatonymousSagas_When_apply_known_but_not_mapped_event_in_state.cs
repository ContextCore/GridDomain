using System;
using CommonDomain;
using GridDomain.Tests.Sagas.SubscriptionRenewSaga.Events;
using NUnit.Framework;

namespace GridDomain.Tests.Sagas.Simplified.Transitions
{
    [TestFixture]
    internal class Given_AutomatonymousSagas_When_apply_known_but_not_mapped_event_in_state
    {

        private readonly Given_AutomatonymousSaga _given = new Given_AutomatonymousSaga(m => m.ChangingSubscription);
        private IAggregate _sagaDataAggregate => _given.SagaDataAggregate;

        private static void When_apply_known_but_not_mapped_event_in_state(SagaInstance<SubscriptionRenewSagaData> sagaInstance)
        {
            sagaInstance.Transit(new SubscriptionExpiredEvent(Guid.NewGuid()));
        }

        [Then]
        public void State_not_changed()
        {
            When_apply_known_but_not_mapped_event_in_state(_given.SagaInstance);
            Assert.AreEqual(_given.SagaMachine.ChangingSubscription, _given.SagaDataAggregate.Data.CurrentState);
        }

        [Then]
        public void State_events_not_raised()
        {
            _sagaDataAggregate.ClearUncommittedEvents();
            When_apply_known_but_not_mapped_event_in_state(_given.SagaInstance);
            CollectionAssert.IsEmpty(_sagaDataAggregate.GetUncommittedEvents());
        }
    }
}