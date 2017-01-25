using System;
using System.Threading.Tasks;
using Akka.Actor;
using GridDomain.CQRS;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Node.Actors;
using GridDomain.Tests.Framework;
using GridDomain.Tests.Unit.Sagas.SoftwareProgrammingDomain.Events;
using NUnit.Framework;

namespace GridDomain.Tests.Unit.Sagas.InstanceSagas
{
    [TestFixture]
    class Given_saga_When_publishing_several_start_messages : SoftwareProgrammingInstanceSagaTest
    {
        private static readonly Guid SagaId = Guid.NewGuid();
        private static  SleptWellEvent secondStartMessage;
        private SagaStateAggregate<SoftwareProgrammingSagaData> SagaData;

        [OneTimeSetUp]
        public async Task When_publishing_start_message()
        {
            var anyMessagePublisher = GridNode.NewDebugWaiter(DefaultTimeout)
                                              .Expect<SagaCreatedEvent<SoftwareProgrammingSagaData>>()
                                              .Create();

            await anyMessagePublisher.SendToSaga(new GotTiredEvent(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), SagaId));

            (await LookupSagaActor<SoftwareProgrammingSaga, SoftwareProgrammingSagaData>(SagaId))
                             .Tell(NotifyOnPersistenceEvents.Instance);

            secondStartMessage = new SleptWellEvent(Guid.NewGuid(), Guid.NewGuid(), SagaId);

            anyMessagePublisher.SendToSaga(secondStartMessage);

            FishForMessage<Persisted>(m => true );

            SagaData = LoadAggregate<SagaStateAggregate<SoftwareProgrammingSagaData>>(SagaId);
        }

        [Then]
        public void Saga_reinitialized_from_last_start_message()
        {
            Assert.AreEqual(secondStartMessage.SofaId, SagaData.Data.SofaId);
        }

        [Then]
        public void Saga_has_correct_state()
        {
            var saga = new SoftwareProgrammingSaga();
            Assert.AreEqual(saga.Coding.Name, SagaData.Data.CurrentStateName);
        }
    }
}