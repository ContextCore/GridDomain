using System;
using GridDomain.CQRS;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Tests.Framework;
using GridDomain.Tests.Sagas.SoftwareProgrammingDomain.Events;
using NUnit.Framework;

namespace GridDomain.Tests.Sagas.InstanceSagas
{
    [TestFixture]
    class Given_saga_When_publishing_any_of_start_messages : SoftwareProgrammingInstanceSagaTest
    {
        private static readonly Guid SagaId = Guid.NewGuid();
        private SagaDataAggregate<SoftwareProgrammingSagaData> _sagaData;

        [OneTimeSetUp]
        public void When_publishing_start_message()
        {
            GridNode.NewDebugWaiter(Timeout)
                    .Expect<SagaCreatedEvent<SoftwareProgrammingSagaData>>()
                    .Create()
                    .Publish(new SleptWellEvent(Guid.NewGuid(), Guid.NewGuid(), SagaId))
                    .Wait();

            _sagaData = LoadAggregate<SagaDataAggregate<SoftwareProgrammingSagaData>>(SagaId);
        }

        [Then]
        public void Saga_data_is_not_null()
        {
            Assert.NotNull(_sagaData.Data);
        }

        [Then]
        public void Saga_has_correct_id()
        {
            Assert.AreEqual(SagaId, _sagaData.Id);
        }
    }
}