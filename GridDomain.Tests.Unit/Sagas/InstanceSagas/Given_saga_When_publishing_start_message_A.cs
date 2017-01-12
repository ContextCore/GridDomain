using System;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Tests.Framework;
using GridDomain.Tests.Unit.Sagas.SoftwareProgrammingDomain.Events;
using NUnit.Framework;

namespace GridDomain.Tests.Unit.Sagas.InstanceSagas
{
    [TestFixture]
    class Given_saga_When_publishing_start_message_A : SoftwareProgrammingInstanceSagaTest
    {
        private static readonly GotTiredEvent StartMessage = 
            new GotTiredEvent(Guid.NewGuid(), 
                              Guid.NewGuid(),
                              Guid.NewGuid(),
                              Guid.NewGuid());

        private SagaStateAggregate<SoftwareProgrammingSagaData> _sagaData;

        [OneTimeSetUp]
        public void When_publishing_start_message()
        {
            GridNode.NewDebugWaiter()
                    .Expect<SagaCreatedEvent<SoftwareProgrammingSagaData>>()
                    .Create()
                    .Publish(StartMessage)
                    .Wait();

            _sagaData = LoadAggregate<SagaStateAggregate<SoftwareProgrammingSagaData>>(StartMessage.SagaId);
        }

        [Then]
        public void Saga_has_correct_data()
        {
            Assert.AreEqual(StartMessage.PersonId, _sagaData.Data.PersonId);
        }

        [Then]
        public void Saga_has_correct_state()
        {
            Assert.AreEqual(nameof(SoftwareProgrammingSaga.MakingCoffee), _sagaData.Data.CurrentStateName);
        }
    }
}
