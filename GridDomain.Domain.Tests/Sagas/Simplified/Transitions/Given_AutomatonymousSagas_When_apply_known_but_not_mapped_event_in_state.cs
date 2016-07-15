using System;
using System.Linq;
using CommonDomain;
using GridDomain.Tests.Sagas.SubscriptionRenewSaga.Events;
using NUnit.Framework;

namespace GridDomain.Tests.Sagas.Simplified.Transitions
{
    [TestFixture]
    internal class Given_AutomatonymousSagas_When_apply_known_but_not_mapped_event_in_state
    {

        public Given_AutomatonymousSagas_When_apply_known_but_not_mapped_event_in_state(Given_AutomatonymousSaga given)
        {
            _given = given;
        }

        public Given_AutomatonymousSagas_When_apply_known_but_not_mapped_event_in_state()
            :this(new Given_AutomatonymousSaga(m => m.ChangingSubscription))
        {
        }

        private readonly Given_AutomatonymousSaga _given;
        private static SubscriptionExpiredEvent _subscriptionExpiredEvent;
        private IAggregate SagaDataAggregate => _given.SagaDataAggregate;

        private static void When_apply_known_but_not_mapped_event_in_state(SagaInstance<SubscriptionRenewSagaData> sagaInstance)
        {
            _subscriptionExpiredEvent = new SubscriptionExpiredEvent(Guid.NewGuid());
            sagaInstance.Transit(_subscriptionExpiredEvent);
        }

        [Then]
        public void State_not_changed()
        {
            When_apply_known_but_not_mapped_event_in_state(_given.SagaInstance);
            Assert.AreEqual(_given.SagaMachine.ChangingSubscription, _given.SagaDataAggregate.Data.CurrentState);
        }

        [Then]
        public void State_events_containes_received_message()
        {
            SagaDataAggregate.ClearUncommittedEvents();
            When_apply_known_but_not_mapped_event_in_state(_given.SagaInstance);
            var @event = SagaDataAggregate.GetUncommittedEvents().OfType<SagaMessageReceivedEvent<SubscriptionRenewSagaData>>().First();
            Assert.AreEqual(_subscriptionExpiredEvent, @event.Message);
        }
    }
}