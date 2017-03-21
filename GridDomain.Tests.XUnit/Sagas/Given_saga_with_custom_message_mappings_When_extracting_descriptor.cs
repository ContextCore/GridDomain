using System.Linq;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Tests.XUnit.Sagas.CustomRoutesSoftwareProgrammingDomain;
using GridDomain.Tests.XUnit.Sagas.SoftwareProgrammingDomain;
using GridDomain.Tests.XUnit.Sagas.SoftwareProgrammingDomain.Commands;
using GridDomain.Tests.XUnit.Sagas.SoftwareProgrammingDomain.Events;
using Xunit;

namespace GridDomain.Tests.XUnit.Sagas
{
    public class Given_saga_with_custom_message_mappings_When_extracting_descriptor
    {
        private readonly ISagaDescriptor _descriptor = CustomRoutesSoftwareProgrammingSaga.Descriptor;

        [Fact]
        public void Descriptor_can_be_created_from_saga()
        {
            Assert.NotNull(_descriptor);
        }

        [Fact]
        public void Descriptor_contains_all_command_types_from_saga()
        {
            var expectedCommands = new[] {typeof(MakeCoffeCommand), typeof(GoSleepCommand)};

            Assert.Equal(expectedCommands, _descriptor.ProduceCommands.ToArray());
        }

        [Fact]
        public void Descriptor_contains_all_domain_event_types_from_saga()
        {
            var expectedEvents = new[]
                                 {
                                     typeof(GotTiredEvent),
                                     typeof(CoffeMadeEvent),
                                     typeof(SleptWellEvent),
                                     typeof(CoffeMakeFailedEvent),
                                     typeof(CustomEvent)
                                 };

            Assert.Equal(expectedEvents, _descriptor.AcceptMessages.Select(m => m.MessageType).ToArray());
        }

        [Fact]
        public void Descriptor_contains_message_start_saga()
        {
            var expected = new[] {typeof(GotTiredEvent), typeof(SleptWellEvent)};
            Assert.Equal(expected, _descriptor.StartMessages.ToArray());
        }

        [Fact]
        public void Descriptor_contains_saga_correlation_field_by_default()
        {
            var expectedEvents = new[]
                                 {
                                     nameof(GotTiredEvent.PersonId),
                                     nameof(CoffeMadeEvent.ForPersonId),
                                     nameof(SleptWellEvent.SofaId),
                                     nameof(CoffeMakeFailedEvent.CoffeMachineId),
                                     nameof(CustomEvent.SagaId)
                                 };

            Assert.Equal(_descriptor.AcceptMessages.Select(m => m.CorrelationField), expectedEvents);
        }

        [Fact]
        public void Descriptor_contains_saga_data_type()
        {
            Assert.Equal(typeof(SagaStateAggregate<SoftwareProgrammingSagaState>), _descriptor.StateType);
        }

        [Fact]
        public void Descriptor_contains_saga_type()
        {
            Assert.Equal(typeof(ISaga<SoftwareProgrammingSagaState>),
                         _descriptor.SagaType);
        }
    }
}