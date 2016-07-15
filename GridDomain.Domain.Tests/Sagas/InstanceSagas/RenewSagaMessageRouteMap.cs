using System;
using System.Runtime.InteropServices;
using GridDomain.CQRS.Messaging;
using GridDomain.CQRS.Messaging.MessageRouting;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Tests.Sagas.InstanceSagas.Commands;
using GridDomain.Tests.Sagas.InstanceSagas.Events;
using NUnit.Framework;

namespace GridDomain.Tests.Sagas.InstanceSagas
{
    public class SoftwareProgrammingSagaRoutes : IMessageRouteMap
    {
        public void Register(IMessagesRouter router)
        {
           // router.RegisterSaga(SoftwareProgrammingSaga.Descriptor);
        }
    }

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
                typeof(GoForCoffeCommand),
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
            Assert.AreEqual(typeof(SoftwareProgrammingSaga), _descriptor.SagaType);
        }

        [Then]
        public void Descriptor_contains_saga_data_type()
        {
            Assert.AreEqual(typeof(SoftwareProgrammingSagaData), _descriptor.StateType);
        }


        [Then]
        public void Descriptor_contains_all_domain_event_types_from_saga()
        {
            var expectedEvents = new []
            {
                typeof(GotTiredDomainEvent),
                typeof(FeltGoodDomainEvent),
                typeof(SleptWellDomainEvent),
                typeof(FeltMoreTiredDomainEvent)
            };

            CollectionAssert.AreEquivalent(_descriptor.AcceptMessages, expectedEvents);
        }
    }
}