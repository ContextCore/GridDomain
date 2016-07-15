using System.Linq;
using CommonDomain;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Sagas;
using GridDomain.Tests.Sagas.SubscriptionRenewSaga.Events;
using NUnit.Framework;

namespace GridDomain.Tests.Sagas.Simplified.Transitions
{

    [TestFixture]
    public class Given_AutomatonymousSaga_When_valid_Transitions
    {
        private readonly Given_AutomatonymousSaga _given = new Given_AutomatonymousSaga(m => m.PayingForSubscription);
        private IAggregate SagaDataAggregate => _given.SagaDataAggregate;

        private static void When_execute_valid_transaction(SagaInstance<SubscriptionRenewSagaData> sagaInstance)
        {
            sagaInstance.Transit(new SubscriptionPaidEvent());
        }

        [Then]
        public void State_raised_creation_event_on_machine_creation()
        {
            var creationEvent = SagaDataAggregate.GetUncommittedEvents().Cast<DomainEvent>().First();
            Assert.IsInstanceOf<SagaCreatedEvent<SubscriptionRenewSagaData>>(creationEvent);
        }

        [Then]
        public void State_is_changed()
        {
            When_execute_valid_transaction(_given.SagaInstance);
            Assert.AreEqual(_given.SagaMachine.SubscriptionSet, _given.SagaInstance.Instance.CurrentState);
        }

        [Then]
        public void State_transitioned_events_are_raised()
        {
            When_execute_valid_transaction(_given.SagaInstance);
            var stateChangeEvent = GetEventAfterTransition();
            Assert.IsInstanceOf<SagaTransitionEvent<SubscriptionRenewSagaData>>(stateChangeEvent);
        }

        [Then]
        public void State_transitioned_events_are_filled_with_sagaId()
        {
            When_execute_valid_transaction(_given.SagaInstance);
            var stateChangeEvent = GetEventAfterTransition();
            Assert.AreEqual(_given.SagaInstance.Data.Id, stateChangeEvent.SourceId);
        }

        [Then]
        public void State_transitioned_events_are_filled_with_sagadata()
        {
            When_execute_valid_transaction(_given.SagaInstance);
            var stateChangeEvent = (SagaTransitionEvent<SubscriptionRenewSagaData>)GetEventAfterTransition();
            Assert.AreEqual(_given.SagaDataAggregate.Data, stateChangeEvent.NewState);
        }

        private DomainEvent GetEventAfterTransition()
        {
            SagaDataAggregate.ClearUncommittedEvents();
            When_execute_valid_transaction(_given.SagaInstance);
            var stateChangeEvent = SagaDataAggregate.GetUncommittedEvents().Cast<DomainEvent>().First();
            return stateChangeEvent;
        }
    }
}
