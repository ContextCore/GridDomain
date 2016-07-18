using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Tests.Sagas.InstanceSagas.Commands;
using GridDomain.Tests.Sagas.InstanceSagas.Events;
using NUnit.Framework;

namespace GridDomain.Tests.Sagas.InstanceSagas
{
    [TestFixture]
    public class Given_saga_When_extracting_descriptor
    {
        private ISagaDescriptor _descriptor;

        [TestFixtureSetUp]
        public void ExtractDescriptor()
        {
            var saga = new SoftwareProgrammingSaga();
            _descriptor = saga.GetDescriptor();
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
            Assert.AreEqual(typeof(GotTiredDomainEvent),_descriptor.StartMessage);
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
                typeof(GotTiredDomainEvent),
                typeof(CoffeMadeDomainEvent),
                typeof(SleptWellDomainEvent),
                typeof(CoffeMakeFailedDomainEvent)
            };

            CollectionAssert.AreEquivalent(_descriptor.AcceptMessages, expectedEvents);
        }
    }
}