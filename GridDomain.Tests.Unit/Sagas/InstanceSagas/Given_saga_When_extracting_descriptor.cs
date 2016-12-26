using System.Linq;
using GridDomain.CQRS;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Tests.Unit.Sagas.SoftwareProgrammingDomain.Commands;
using GridDomain.Tests.Unit.Sagas.SoftwareProgrammingDomain.Events;
using NUnit.Framework;

namespace GridDomain.Tests.Unit.Sagas.InstanceSagas
{
    [TestFixture]
    public class Given_saga_When_extracting_descriptor
    {
        private ISagaDescriptor _descriptor;

        [OneTimeSetUp]
        public void ExtractDescriptor()
        {
            var saga = new SoftwareProgrammingSaga();
            _descriptor = saga.CreateDescriptor<SoftwareProgrammingSaga,SoftwareProgrammingSagaData>(typeof(GotTiredEvent),typeof(SleptWellEvent));
        }

        [Then]
        public void Descriptor_can_be_created_from_saga()
        {
            Assert.NotNull(_descriptor);
        }

        [Then]
        public void Descriptor_contains_all_command_types_from_saga()
        {
            var expectedCommands = new[]
            {
                typeof(MakeCoffeCommand),
                typeof(GoSleepCommand)
            };

            CollectionAssert.AreEquivalent(expectedCommands,_descriptor.ProduceCommands);
        }

        [Then]
        public void Descriptor_contains_message_start_saga()
        {
            CollectionAssert.AreEquivalent(new[] {typeof(GotTiredEvent), typeof(SleptWellEvent)}, _descriptor.StartMessages);
        }

        [Then]
        public void Descriptor_contains_saga_type()
        {
            Assert.AreEqual(typeof(ISagaInstance<SoftwareProgrammingSaga,SoftwareProgrammingSagaData>), _descriptor.SagaType);
        }

        [Then]
        public void Descriptor_contains_saga_data_type()
        {
            Assert.AreEqual(typeof(SagaDataAggregate<SoftwareProgrammingSagaData>), _descriptor.StateType);
        }


        [Then]
        public void Descriptor_contains_all_domain_event_types_from_saga()
        {
            var expectedEvents = new []
            {
                typeof(GotTiredEvent),
                typeof(CoffeMadeEvent),
                typeof(SleptWellEvent),
                typeof(CoffeMakeFailedEvent),
                typeof(Fault<GoSleepCommand>)
            };

            CollectionAssert.AreEquivalent(_descriptor.AcceptMessages.Select(m => m.MessageType), expectedEvents);
        }

        [Then]
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

            CollectionAssert.AreEquivalent(_descriptor.AcceptMessages.Select(m => m.CorrelationField), expectedEvents);
        }
    }
}