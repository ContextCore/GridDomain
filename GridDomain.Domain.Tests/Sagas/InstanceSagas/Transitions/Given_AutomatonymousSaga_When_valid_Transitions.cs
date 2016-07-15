using System;
using System.Linq;
using CommonDomain;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Tests.Sagas.StateSagas.SampleSaga.Events;
using NUnit.Framework;

namespace GridDomain.Tests.Sagas.InstanceSagas.Transitions
{
    [TestFixture]
    public class Given_AutomatonymousSaga_When_valid_Transitions
    {
        private static void When_execute_valid_transaction<T>(SagaInstance<SubscriptionRenewSagaData> sagaInstance, T e = null) where T:DomainEvent
        {
                sagaInstance.Transit(e);
        }

        [Then]
        public void State_raised_creation_event_on_machine_creation()
        {
            var given = new Given_AutomatonymousSaga(m => m.PayingForSubscription);
            var creationEvent = ExtractFirstEvent<DomainEvent>(given.SagaDataAggregate);
            Assert.IsInstanceOf<SagaCreatedEvent<SubscriptionRenewSagaData>>(creationEvent);
        }

        [Then]
        public void State_is_changed()
        {
            var given = new Given_AutomatonymousSaga(m => m.PayingForSubscription);
            When_execute_valid_transaction(given.SagaInstance, new SubscriptionPaidEvent(Guid.NewGuid()));
            Assert.AreEqual(given.SagaMachine.SubscriptionSet, given.SagaDataAggregate.Data.CurrentState);
        }

        [Then]
        public void State_transitioned_event_is_raised()
        {
            var given = new Given_AutomatonymousSaga(m => m.PayingForSubscription);
            ClearEvents(given.SagaDataAggregate);
            When_execute_valid_transaction(given.SagaInstance, new SubscriptionPaidEvent(Guid.NewGuid()));

            var stateChangeEvent = GetFirstOf<SagaTransitionEvent<SubscriptionRenewSagaData>>(given.SagaDataAggregate);
            Assert.NotNull(stateChangeEvent);
        }

        [Then]
        public void Message_received_event_is_raised()
        {
            var given = new Given_AutomatonymousSaga(m => m.SubscriptionSet);
            ClearEvents(given.SagaDataAggregate);
            When_execute_valid_transaction(given.SagaInstance, new SubscriptionExpiredEvent(Guid.NewGuid()));

            var messageReceivedEvent = GetFirstOf<SagaMessageReceivedEvent<SubscriptionRenewSagaData>>(given.SagaDataAggregate);
            Assert.NotNull(messageReceivedEvent);
        }

        [Then]
        public void State_transition_event_is_before_Message_received_event()
        {
            var given = new Given_AutomatonymousSaga(m => m.PayingForSubscription);
            ClearEvents(given.SagaDataAggregate);
            When_execute_valid_transaction(given.SagaInstance, new SubscriptionPaidEvent(Guid.NewGuid()));

            var allEvents = GetAllAs<DomainEvent>(given.SagaDataAggregate);
            var messageReceivedEvent = GetFirstOf<SagaMessageReceivedEvent<SubscriptionRenewSagaData>>(given.SagaDataAggregate);
            var stateTransitionEvent = GetFirstOf<SagaTransitionEvent<SubscriptionRenewSagaData>>(given.SagaDataAggregate);

            int indexOfTransition = Array.IndexOf(allEvents,stateTransitionEvent);
            int indexOfMessageReceived = Array.IndexOf(allEvents, messageReceivedEvent);

            Assert.True(indexOfMessageReceived == indexOfTransition - 1,"message received event is not followed by state transition event");
        }

        [Then]
        public void Message_received_event_contains_incoming_message()
        {
            var given = new Given_AutomatonymousSaga(m => m.PayingForSubscription);
            ClearEvents(given.SagaDataAggregate);
            var message = new SubscriptionPaidEvent(Guid.NewGuid());
            When_execute_valid_transaction(given.SagaInstance,message);
            var stateChangeEvent = GetFirstOf<SagaMessageReceivedEvent<SubscriptionRenewSagaData>>(given.SagaDataAggregate);
            Assert.AreEqual(message, stateChangeEvent.Message);
        }

        [Then]
        public void Message_received_event_is_filled_with_sagaData_after_event_apply_to_it()
        {
            var given = new Given_AutomatonymousSaga(m => m.SubscriptionSet);

            ClearEvents(given.SagaDataAggregate);

            When_execute_valid_transaction(given.SagaInstance, new SubscriptionExpiredEvent(Guid.NewGuid()));
            var changedSubscriptionId = given.SagaDataAggregate.Data.SubscriptionId = Guid.NewGuid();

            var messageReceivedEvent =
                GetFirstOf<SagaMessageReceivedEvent<SubscriptionRenewSagaData>>(given.SagaDataAggregate);

            Assert.AreEqual(changedSubscriptionId, messageReceivedEvent.SagaData.SubscriptionId);
        }

        [Then]
        public void Commands_are_produced()
        {
            var given = new Given_AutomatonymousSaga(m => m.SubscriptionSet);

            var subscriptionExpiredEvent = new SubscriptionExpiredEvent(Guid.NewGuid());
            When_execute_valid_transaction(given.SagaInstance, subscriptionExpiredEvent);

            CollectionAssert.IsNotEmpty(given.SagaInstance.CommandsToDispatch);
        }

        [Then]
        public void SagaData_is_changed_after_transition_by_event_data()
        {
            var given = new Given_AutomatonymousSaga(m => m.SubscriptionSet);
            ClearEvents(given.SagaDataAggregate);

            var subscriptionExpiredEvent = new SubscriptionExpiredEvent(Guid.NewGuid());
            When_execute_valid_transaction(given.SagaInstance, subscriptionExpiredEvent);

            var newSubscriptionId = given.SagaDataAggregate.Data.SubscriptionId;

            Assert.AreEqual(subscriptionExpiredEvent.SourceId, newSubscriptionId);
        }

        [Then]
        public void State_transitioned_events_are_filled_with_sagadata()
        {
            var given = new Given_AutomatonymousSaga(m => m.PayingForSubscription);
            ClearEvents(given.SagaDataAggregate);
            When_execute_valid_transaction(given.SagaInstance, new SubscriptionPaidEvent(Guid.NewGuid()));
            var stateChangeEvent = GetFirstOf<SagaTransitionEvent<SubscriptionRenewSagaData>>(given.SagaDataAggregate);
            Assert.AreEqual(given.SagaDataAggregate.Data, stateChangeEvent.SagaData);
        }


        [Then]
        public void Message_received_events_are_filled_with_sagadata()
        {
            var given = new Given_AutomatonymousSaga(m => m.PayingForSubscription);
            ClearEvents(given.SagaDataAggregate);
            When_execute_valid_transaction(given.SagaInstance, new SubscriptionPaidEvent(Guid.NewGuid()));
            var stateChangeEvent = GetFirstOf<SagaMessageReceivedEvent<SubscriptionRenewSagaData>>(given.SagaDataAggregate);
            Assert.AreEqual(given.SagaDataAggregate.Data, stateChangeEvent.SagaData);
        }

        private T ExtractFirstEvent<T>(IAggregate sagaDataAggregate)
        {
            return GetAllAs<T>(sagaDataAggregate).First();
        }

        private T GetFirstOf<T>(IAggregate sagaDataAggregate)
        {
            var stateChangeEvent = sagaDataAggregate.GetUncommittedEvents().OfType<T>().First();
            return stateChangeEvent;
        }
        private T[] GetAllOf<T>(IAggregate sagaDataAggregate)
        {
            var stateChangeEvent = sagaDataAggregate.GetUncommittedEvents().OfType<T>().ToArray();
            return stateChangeEvent;
        }

        private T[] GetAllAs<T>(IAggregate sagaDataAggregate)
        {
            var stateChangeEvent = sagaDataAggregate.GetUncommittedEvents().Cast<T>().ToArray();
            return stateChangeEvent;
        }

        private void ClearEvents(IAggregate agr)
        {
            agr.ClearUncommittedEvents();
        }
    }
}
