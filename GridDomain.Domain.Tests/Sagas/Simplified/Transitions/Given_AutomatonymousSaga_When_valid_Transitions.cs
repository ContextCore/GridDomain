using System;
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
        private static void When_execute_valid_transaction(SagaInstance<SubscriptionRenewSagaData> sagaInstance, SubscriptionPaidEvent e = null)
        {
            var subscriptionPaidEvent = new SubscriptionPaidEvent();
            sagaInstance.Transit(e ?? subscriptionPaidEvent);
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
            When_execute_valid_transaction(given.SagaInstance);
            Assert.AreEqual(given.SagaMachine.SubscriptionSet, given.SagaDataAggregate.Data.CurrentState);
        }

        [Then]
        public void State_transitioned_event_is_raised()
        {
            var given = new Given_AutomatonymousSaga(m => m.PayingForSubscription);
            ClearEvents(given.SagaDataAggregate);
            When_execute_valid_transaction(given.SagaInstance);
            var stateChangeEvent = ExtractFirstEvent<DomainEvent>(given.SagaDataAggregate);
            Assert.IsInstanceOf<SagaTransitionEvent<SubscriptionRenewSagaData>>(stateChangeEvent);
        }

        [Then]
        public void Event_received_event_is_raised()
        {
            throw new NotImplementedException();
        }

        [Then]
        public void State_transition_event_is_before_Event_received_event()
        {
            throw new NotImplementedException();
        }


        [Then]
        public void State_event_recieved_events_contains_incoming_message()
        {
            var given = new Given_AutomatonymousSaga(m => m.PayingForSubscription);
            ClearEvents(given.SagaDataAggregate);
            var message = new SubscriptionPaidEvent();
            When_execute_valid_transaction(given.SagaInstance,message);
            var stateChangeEvent = ExtractFirstEvent<SagaEventReceivedEvent<SubscriptionRenewSagaData>>(given.SagaDataAggregate);
            Assert.AreEqual(message, stateChangeEvent.Message);
        }

        [Then]
        public void Event_received_event_is_filled_with_sagaData_before_event_apply_to_it()
        {
            throw new NotImplementedException();
        }

        [Then]
        public void State_transitioned_events_are_filled_with_sagadata()
        {
            var given = new Given_AutomatonymousSaga(m => m.PayingForSubscription);
            ClearEvents(given.SagaDataAggregate);
            When_execute_valid_transaction(given.SagaInstance);
            var stateChangeEvent = ExtractFirstEvent<SagaTransitionEvent<SubscriptionRenewSagaData>>(given.SagaDataAggregate);
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
