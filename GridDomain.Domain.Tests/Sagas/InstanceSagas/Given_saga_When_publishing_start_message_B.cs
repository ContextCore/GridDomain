using System;
using GridDomain.CQRS;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Tests.Framework;
using GridDomain.Tests.Sagas.SoftwareProgrammingDomain.Events;
using NUnit.Framework;

namespace GridDomain.Tests.Sagas.InstanceSagas
{
    [TestFixture]
    class Given_saga_When_publishing_start_message_B : SoftwareProgrammingInstanceSagaTest
    {
        private static readonly SleptWellEvent StartMessage = new SleptWellEvent(Guid.NewGuid(),Guid.NewGuid(),Guid.NewGuid(), null);
        private SagaDataAggregate<SoftwareProgrammingSagaData> _sagaData;

        [OneTimeSetUp]
        public void When_publishing_start_message()
        {
            GridNode.NewDebugWaiter()
                    .Expect<SagaCreatedEvent<SoftwareProgrammingSagaData>>()
                    .Create()
                    .Publish(StartMessage)
                    .Wait();

            _sagaData = LoadAggregate<SagaDataAggregate<SoftwareProgrammingSagaData>>(StartMessage.SagaId);
        }

        [Then]
        public void Saga_has_correct_data()
        {
            Assert.AreEqual(StartMessage.SofaId, _sagaData.Data.SofaId);
        }

        [Then]
        public void Saga_has_correct_state()
        {
            Assert.AreEqual(nameof(SoftwareProgrammingSaga.Coding), _sagaData.Data.CurrentStateName);
        }
    }
}