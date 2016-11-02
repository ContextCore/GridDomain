using System;
using System.Linq;
using System.Threading;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Tests.Sagas.SoftwareProgrammingDomain;
using GridDomain.Tests.Sagas.SoftwareProgrammingDomain.Commands;
using GridDomain.Tests.Sagas.SoftwareProgrammingDomain.Events;
using NUnit.Framework;

namespace GridDomain.Tests.Sagas.InstanceSagas
{
    [TestFixture]
    class Given_saga_When_handling_command_faults : SoftwareProgrammingInstanceSagaTest
    {
        private SagaDataAggregate<SoftwareProgrammingSagaData> _sagaDataAggregate;
        private CoffeMakeFailedEvent _coffeMakeFailedEvent;
        private SoftwareProgrammingSagaData _sagaData;

        protected override IContainerConfiguration CreateConfiguration()
        {
            return new CustomContainerConfiguration(c => base.CreateConfiguration().Register(c), c => c.RegisterAggregate<HomeAggregate,HomeAggregateHandler>());
        }

        protected override IMessageRouteMap CreateMap()
        {
            return new CustomRouteMap(r => base.CreateMap().Register(r),
                                      r => r.RegisterAggregate(HomeAggregateHandler.Descriptor));
        }

        [OneTimeSetUp]
        public void When_publishing_start_message()
        {
            var sagaId = Guid.NewGuid();
            _sagaData = new SoftwareProgrammingSagaData(nameof(SoftwareProgrammingSaga.MakingCoffee))
            {
               PersonId = Guid.NewGuid()
            };

            var sagaDataEvent = new SagaCreatedEvent<SoftwareProgrammingSagaData>(_sagaData, sagaId);

            SaveInJournal<SagaDataAggregate<SoftwareProgrammingSagaData>>(sagaId,sagaDataEvent);

            Thread.Sleep(100);
            _coffeMakeFailedEvent = new CoffeMakeFailedEvent(Guid.NewGuid(), Guid.NewGuid(), BusinessDateTime.UtcNow,sagaId);

            GridNode.Transport.Publish(_coffeMakeFailedEvent);

            //WaitFor<SagaTransitionEvent<SoftwareProgrammingSagaData>>();
            //WaitFor<SagaTransitionEvent<SoftwareProgrammingSagaData>>();
            Thread.Sleep(1000);
            _sagaDataAggregate = LoadAggregate<SagaDataAggregate<SoftwareProgrammingSagaData>>(sagaId);
        }

        [Then]
        public void Saga_should_be_in_correct_state_after_fault_handling()
        {
            Assert.AreEqual(nameof(SoftwareProgrammingSaga.MakingCoffee), _sagaDataAggregate.Data.CurrentStateName);
        }

        [Then]
        public void Saga_should_receive_fault_message()
        {
            CollectionAssert.IsNotEmpty(_sagaDataAggregate.ReceivedMessages.OfType<IFault<GoSleepCommand>>());
        }

        [Then]
        public void Saga_state_should_contain_data_from_fault_message()
        {
            var fault = _sagaDataAggregate.ReceivedMessages.OfType<IFault<GoSleepCommand>>().First();
            Assert.AreEqual(_sagaData.SofaId, fault.Message.SofaId);
        }
    }
}