using System;
using System.Linq;
using System.Threading;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Tests.Framework;
using GridDomain.Tests.Unit.Sagas.SoftwareProgrammingDomain;
using GridDomain.Tests.Unit.Sagas.SoftwareProgrammingDomain.Commands;
using GridDomain.Tests.Unit.Sagas.SoftwareProgrammingDomain.Events;
using NUnit.Framework;

namespace GridDomain.Tests.Unit.Sagas.InstanceSagas
{
    [TestFixture]
    class Given_saga_When_handling_command_faults : SoftwareProgrammingInstanceSagaTest
    {
        private SagaStateAggregate<SoftwareProgrammingSagaData> _sagaDataAggregate;
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
            _sagaData = new SoftwareProgrammingSagaData(sagaId,nameof(SoftwareProgrammingSaga.MakingCoffee))
            {
               PersonId = Guid.NewGuid()
            };

            var sagaDataEvent = new SagaCreatedEvent<SoftwareProgrammingSagaData>(_sagaData, sagaId);

            SaveInJournal<SagaStateAggregate<SoftwareProgrammingSagaData>>(sagaId,sagaDataEvent);

            Thread.Sleep(100);
            _coffeMakeFailedEvent = new CoffeMakeFailedEvent(Guid.NewGuid(), Guid.NewGuid(), BusinessDateTime.UtcNow,sagaId);

            GridNode.NewDebugWaiter()
                    .Expect<object>()
                    .Create()
                    .SendToSaga(_coffeMakeFailedEvent, new MessageMetadata(_coffeMakeFailedEvent.SourceId));

            Thread.Sleep(1000);
            _sagaDataAggregate = LoadAggregate<SagaStateAggregate<SoftwareProgrammingSagaData>>(sagaId);
        }

        [Then]
        public void Saga_should_be_in_correct_state_after_fault_handling()
        {
            Assert.AreEqual(nameof(SoftwareProgrammingSaga.Coding), _sagaDataAggregate.Data.CurrentStateName);
        }

        [Then]
        public void Saga_state_should_contain_data_from_fault_message()
        {
            Assert.AreEqual(_coffeMakeFailedEvent.ForPersonId, _sagaData.BadSleepPersonId);
        }
    }
}