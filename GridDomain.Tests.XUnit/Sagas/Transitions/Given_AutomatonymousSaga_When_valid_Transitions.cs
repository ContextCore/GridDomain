using System;
using System.Linq;
using System.Threading.Tasks;
using CommonDomain;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Tests.XUnit.CommandsExecution;
using GridDomain.Tests.XUnit.Sagas.SoftwareProgrammingDomain;
using GridDomain.Tests.XUnit.Sagas.SoftwareProgrammingDomain.Events;
using Serilog;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.XUnit.Sagas.Transitions
{
    public class Given_AutomatonymousSaga_When_valid_Transitions
    {
        public Given_AutomatonymousSaga_When_valid_Transitions(ITestOutputHelper output)
        {
            log = new XUnitAutoTestLoggerConfiguration(output).CreateLogger();
        }

        private static void When_execute_valid_transaction<T>(ISagaInstance sagaInstance, T e = null) where T : DomainEvent
        {
            sagaInstance.Transit(e);
        }

        private readonly ILogger log;

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

        [Fact]
        public void Commands_are_produced()
        {
            var given = new Given_AutomatonymousSaga(m => m.Coding, log);

            var subscriptionExpiredEvent = new GotTiredEvent(Guid.NewGuid());
            When_execute_valid_transaction(given.SagaInstance, subscriptionExpiredEvent);

            Assert.NotEmpty(given.SagaInstance.CommandsToDispatch);
        }

        [Fact]
        public void Message_received_event_contains_incoming_message()
        {
            var given = new Given_AutomatonymousSaga(m => m.MakingCoffee, log);
            ClearEvents(given.SagaDataAggregate);
            var message = new CoffeMadeEvent(Guid.NewGuid(), Guid.NewGuid());
            When_execute_valid_transaction(given.SagaInstance, message);
            var stateChangeEvent = GetFirstOf<SagaMessageReceivedEvent<SoftwareProgrammingSagaData>>(given.SagaDataAggregate);
            Assert.Equal(message, stateChangeEvent.Message);
        }

        [Fact]
        public void Message_received_event_is_filled_with_sagaData_after_event_apply_to_it()
        {
            var given = new Given_AutomatonymousSaga(m => m.Coding, log);

            ClearEvents(given.SagaDataAggregate);

            When_execute_valid_transaction(given.SagaInstance, new GotTiredEvent(Guid.NewGuid()));
            var changedSubscriptionId = given.SagaDataAggregate.Data.PersonId = Guid.NewGuid();

            var messageReceivedEvent =
                GetFirstOf<SagaMessageReceivedEvent<SoftwareProgrammingSagaData>>(given.SagaDataAggregate);

            Assert.Equal(changedSubscriptionId, messageReceivedEvent.SagaData.PersonId);
        }

        [Fact]
        public void Message_received_event_is_raised()
        {
            var given = new Given_AutomatonymousSaga(m => m.Coding, log);
            ClearEvents(given.SagaDataAggregate);
            When_execute_valid_transaction(given.SagaInstance, new GotTiredEvent(Guid.NewGuid()));

            var messageReceivedEvent =
                GetFirstOf<SagaMessageReceivedEvent<SoftwareProgrammingSagaData>>(given.SagaDataAggregate);
            Assert.NotNull(messageReceivedEvent);
        }

        [Fact]
        public void Message_received_events_are_filled_with_sagadata()
        {
            var given = new Given_AutomatonymousSaga(m => m.MakingCoffee, log);
            ClearEvents(given.SagaDataAggregate);
            When_execute_valid_transaction(given.SagaInstance, new CoffeMadeEvent(Guid.NewGuid(), Guid.NewGuid()));
            var stateChangeEvent = GetFirstOf<SagaMessageReceivedEvent<SoftwareProgrammingSagaData>>(given.SagaDataAggregate);
            Assert.Equal(given.SagaDataAggregate.Data, stateChangeEvent.SagaData);
        }

        [Fact]
        public void SagaData_is_changed_after_transition_by_event_data()
        {
            var given = new Given_AutomatonymousSaga(m => m.Coding, log);
            ClearEvents(given.SagaDataAggregate);

            var subscriptionExpiredEvent = new GotTiredEvent(Guid.NewGuid());
            When_execute_valid_transaction(given.SagaInstance, subscriptionExpiredEvent);

            var newSubscriptionId = given.SagaDataAggregate.Data.PersonId;

            Assert.Equal(subscriptionExpiredEvent.SourceId, newSubscriptionId);
        }

        [Fact]
        public void State_is_changed()
        {
            var given = new Given_AutomatonymousSaga(m => m.MakingCoffee, log);
            When_execute_valid_transaction(given.SagaInstance, new CoffeMadeEvent(Guid.NewGuid(), Guid.NewGuid()));
            Assert.Equal(given.SagaMachine.Coding.Name, given.SagaDataAggregate.Data.CurrentStateName);
        }

        [Fact]
        public void State_is_changed_on_using_non_generic_transit_method()
        {
            var given = new Given_AutomatonymousSaga(m => m.MakingCoffee, log);
            object msg = new CoffeMadeEvent(Guid.NewGuid(), Guid.NewGuid());
            given.SagaInstance.Transit((dynamic) msg);
            Assert.Equal(given.SagaMachine.Coding.Name, given.SagaDataAggregate.Data.CurrentStateName);
        }

        [Fact]
        public void State_raised_creation_event_on_machine_creation()
        {
            var given = new Given_AutomatonymousSaga(m => m.MakingCoffee, log);
            var creationEvent = ExtractFirstEvent<DomainEvent>(given.SagaDataAggregate);
            Assert.IsAssignableFrom<SagaCreatedEvent<SoftwareProgrammingSagaData>>(creationEvent);
        }

        [Fact]
        public async Task When_apply_known_but_not_mapped_event_in_state()
        {
            var given = new Given_AutomatonymousSaga(m => m.Sleeping, log);
            var gotTiredEvent = new GotTiredEvent(Guid.NewGuid());
            //Transition_raises_an_error()
            await given.SagaInstance.Transit(gotTiredEvent).ShouldThrow<SagaTransitionException>();
        }
    }
}