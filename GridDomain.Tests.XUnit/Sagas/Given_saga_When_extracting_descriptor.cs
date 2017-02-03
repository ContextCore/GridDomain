using System.Linq;
using GridDomain.CQRS;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Tests.XUnit.Sagas.SoftwareProgrammingDomain;
using GridDomain.Tests.XUnit.Sagas.SoftwareProgrammingDomain.Commands;
using GridDomain.Tests.XUnit.Sagas.SoftwareProgrammingDomain.Events;
using Xunit;

namespace GridDomain.Tests.XUnit.Sagas
{
   
    public class Given_saga_When_extracting_descriptor
    {
        private readonly ISagaDescriptor _descriptor = SoftwareProgrammingSaga.Descriptor;

       [Fact]
        public void Descriptor_can_be_created_from_saga()
        {
            Assert.NotNull(_descriptor);
        }

      [Fact]
        public void Descriptor_contains_all_command_types_from_saga()
        {
            var expectedCommands = new[]
            {
                typeof(MakeCoffeCommand),
                typeof(GoSleepCommand)
            };

            Assert.Equal(expectedCommands,_descriptor.ProduceCommands.ToArray());
        }

      [Fact]
        public void Descriptor_contains_message_start_saga()
        {
            Assert.Equal(new[] {typeof(GotTiredEvent), typeof(SleptWellEvent)}, _descriptor.StartMessages.ToArray());
        }

      [Fact]
        public void Descriptor_contains_saga_type()
        {
            Assert.Equal(typeof(ISagaInstance<SoftwareProgrammingSaga,SoftwareProgrammingSagaData>), _descriptor.SagaType);
        }

      [Fact]
        public void Descriptor_contains_saga_data_type()
        {
            Assert.Equal(typeof(SagaStateAggregate<SoftwareProgrammingSagaData>), _descriptor.StateType);
        }


      [Fact]
        public void Descriptor_contains_all_domain_event_types_from_saga()
        {
            var expectedEvents = new []
            {
                typeof(GotTiredEvent),
                typeof(CoffeMadeEvent),
                typeof(SleptWellEvent),
                typeof(Fault<GoSleepCommand>),
                typeof(CoffeMakeFailedEvent),
            };

            Assert.Equal(expectedEvents, _descriptor.AcceptMessages.Select(m => m.MessageType).ToArray());
        }

      [Fact]
        public void Descriptor_contains_saga_correlation_field_by_default()
        {
            var expectedEvents = new[]
            {
                nameof(DomainEvent.SagaId),
                nameof(DomainEvent.SagaId),
                nameof(DomainEvent.SagaId),
                nameof(DomainEvent.SagaId),
                nameof(Fault.SagaId),
            };

            Assert.Equal(expectedEvents, _descriptor.AcceptMessages.Select(m => m.CorrelationField));
        }
    }
}