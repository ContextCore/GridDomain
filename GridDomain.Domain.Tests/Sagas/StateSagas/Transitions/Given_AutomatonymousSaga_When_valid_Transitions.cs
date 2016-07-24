using System;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Tests.Framework;
using GridDomain.Tests.Sagas.InstanceSagas;
using GridDomain.Tests.Sagas.SoftwareProgrammingDomain.Events;
using NUnit.Framework;
using SoftwareProgrammingSaga = GridDomain.Tests.Sagas.StateSagas.SampleSaga.SoftwareProgrammingSaga;

namespace GridDomain.Tests.Sagas.StateSagas.Transitions
{
    [TestFixture]
    public class Given_AutomatonymousSaga_When_valid_Transitions
    {
        private static void When_execute_valid_transaction<T>(ISagaInstance sagaInstance, T e = null) where T:DomainEvent
        {
                sagaInstance.Transit(e);
        }

        [Then]
        public void State_raised_creation_event_on_machine_creation()
        {
            var given = new Given_State_SoftareProgramming_Saga(
                SoftwareProgrammingSaga.States.MakingCoffe);

            var creationEvent = given.SagaDataAggregate.GetEvent<DomainEvent>();
            Assert.IsInstanceOf<SagaCreatedEvent<SoftwareProgrammingSaga.States>>(creationEvent);
        }

        [Then]
        public void State_is_changed()
        {
            var given = new Given_State_SoftareProgramming_Saga(SoftwareProgrammingSaga.States.MakingCoffe);

            When_execute_valid_transaction(given.SagaMachine, new CoffeMadeEvent(Guid.NewGuid(), Guid.NewGuid()));
            Assert.AreEqual(SoftwareProgrammingSaga.States.Working, given.SagaMachine.State.MachineState);
        }

        [Then]
        public void State_is_changed_on_using_non_generic_transit_method()
        {
            var given = new Given_State_SoftareProgramming_Saga(SoftwareProgrammingSaga.States.MakingCoffe);

            object msg = new CoffeMadeEvent(Guid.NewGuid(), Guid.NewGuid());
            given.SagaMachine.Transit(msg);
            Assert.AreEqual(SoftwareProgrammingSaga.States.Working, given.SagaMachine.State.MachineState);
        }

        [Then]
        public void State_transitioned_event_is_raised()
        {
            var given = new Given_State_SoftareProgramming_Saga(SoftwareProgrammingSaga.States.MakingCoffe);

            given.SagaDataAggregate.ClearEvents();
            When_execute_valid_transaction(given.SagaMachine, new CoffeMadeEvent(Guid.NewGuid(), Guid.NewGuid()));

            var stateChangeEvent = given.SagaDataAggregate.GetEvent<SagaTransitionEvent<SoftwareProgrammingSaga.Triggers>>();
            Assert.NotNull(stateChangeEvent);
        }

        [Then]
        public void Message_received_event_is_raised()
        {
            var given = new Given_State_SoftareProgramming_Saga(SoftwareProgrammingSaga.States.Working);

            given.SagaDataAggregate.ClearEvents();
            When_execute_valid_transaction(given.SagaMachine, new GotTiredEvent(Guid.NewGuid()));

            var messageReceivedEvent = given.SagaDataAggregate.GetEvent<SagaMessageReceivedEvent<SoftwareProgrammingSaga.States>>();
            Assert.NotNull(messageReceivedEvent);
        }

        [Then]
        public void State_transition_event_is_before_Message_received_event()
        {
            var given = new Given_State_SoftareProgramming_Saga(SoftwareProgrammingSaga.States.MakingCoffe);

            given.SagaDataAggregate.ClearEvents();
            When_execute_valid_transaction(given.SagaMachine, new CoffeMadeEvent(Guid.NewGuid(), Guid.NewGuid()));

            var allEvents = given.SagaDataAggregate.GetEvents<DomainEvent>();
            var messageReceivedEvent = given.SagaDataAggregate.GetEvent<SagaMessageReceivedEvent<SoftwareProgrammingSaga.States>>();
            var stateTransitionEvent = given.SagaDataAggregate.GetEvent<SagaTransitionEvent<SoftwareProgrammingSaga.States>>();

            int indexOfTransition = Array.IndexOf(allEvents,stateTransitionEvent);
            int indexOfMessageReceived = Array.IndexOf(allEvents, messageReceivedEvent);

            Assert.True(indexOfMessageReceived == indexOfTransition - 1,"message received event is not followed by state transition event");
        }

        [Then]
        public void Message_received_event_contains_incoming_message()
        {
            var given = new Given_State_SoftareProgramming_Saga(SoftwareProgrammingSaga.States.MakingCoffe);

            given.SagaDataAggregate.ClearEvents();
            var message = new CoffeMadeEvent(Guid.NewGuid(), Guid.NewGuid());
            When_execute_valid_transaction(given.SagaMachine,message);
            var stateChangeEvent = given.SagaDataAggregate.GetEvent<SagaMessageReceivedEvent<SoftwareProgrammingSaga.States>>();
            Assert.AreEqual(message, stateChangeEvent.Message);
        }

        [Then]
        public void Message_received_event_is_filled_with_sagaData_after_event_apply_to_it()
        {
            var given = new Given_State_SoftareProgramming_Saga(SoftwareProgrammingSaga.States.MakingCoffe);

            given.SagaDataAggregate.ClearEvents();

            When_execute_valid_transaction(given.SagaMachine, new GotTiredEvent(Guid.NewGuid()));
            var changedSubscriptionId  = Guid.NewGuid();

            var messageReceivedEvent =
                given.SagaDataAggregate.GetEvent<SagaMessageReceivedEvent<SoftwareProgrammingSagaData>>();

            Assert.AreEqual(changedSubscriptionId, messageReceivedEvent.SagaData.PersonId);
        }

        [Then]
        public void Commands_are_produced()
        {
            var given = new Given_State_SoftareProgramming_Saga(SoftwareProgrammingSaga.States.Working);

            var subscriptionExpiredEvent = new GotTiredEvent(Guid.NewGuid());
            When_execute_valid_transaction(given.SagaInstance, subscriptionExpiredEvent);

            CollectionAssert.IsNotEmpty(given.SagaInstance.CommandsToDispatch);
        }

        [Then]
        public void SagaData_is_changed_after_transition_by_event_data()
        {
            var given = new Given_State_SoftareProgramming_Saga(SoftwareProgrammingSaga.States.Working);
            given.SagaDataAggregate.ClearEvents();

            var gotTiredEvent = new GotTiredEvent(Guid.NewGuid());
            When_execute_valid_transaction(given.SagaInstance, gotTiredEvent);

            var personId = given.SagaDataAggregate.PersonId;

            Assert.AreEqual(gotTiredEvent.SourceId, personId);
        }

        [Then]
        public void State_transitioned_events_are_filled_with_sagadata()
        {
            var given = new Given_State_SoftareProgramming_Saga(SoftwareProgrammingSaga.States.MakingCoffe);
            given.SagaDataAggregate.ClearEvents();
            When_execute_valid_transaction(given.SagaInstance, new CoffeMadeEvent(Guid.NewGuid(),Guid.NewGuid()));
            var stateChangeEvent = given.SagaDataAggregate.GetEvent<SagaTransitionEvent<SoftwareProgrammingSaga.States>>();
            Assert.AreEqual(given.SagaDataAggregate.MachineState, stateChangeEvent.SagaData);
        }

        [Then]
        public void Message_received_events_are_filled_with_sagadata()
        {
            var given = new Given_State_SoftareProgramming_Saga(SoftwareProgrammingSaga.States.MakingCoffe);
            given.SagaDataAggregate.ClearEvents();
            When_execute_valid_transaction(given.SagaInstance, new CoffeMadeEvent(Guid.NewGuid(),Guid.NewGuid()));
            var stateChangeEvent = given.SagaDataAggregate.GetEvent<SagaMessageReceivedEvent<SoftwareProgrammingSaga.States>>();
            Assert.AreEqual(given.SagaDataAggregate.MachineState, stateChangeEvent.SagaData);
        }
    }
}
