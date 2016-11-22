using System;
using System.Threading;
using Akka.Actor;
using Akka.Persistence;
using GridDomain.CQRS;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Node.Actors;
using GridDomain.Tests.Framework;
using GridDomain.Tests.Sagas.SoftwareProgrammingDomain.Events;
using NUnit.Framework;

namespace GridDomain.Tests.Sagas.InstanceSagas
{
    [TestFixture]
    class Given_saga_When_publishing_several_start_messages : SoftwareProgrammingInstanceSagaTest
    {
        private static readonly Guid SagaId = Guid.NewGuid();
        private static  SleptWellEvent secondStartMessage;
        private SagaDataAggregate<SoftwareProgrammingSagaData> SagaData;

        [OneTimeSetUp]
        public void When_publishing_start_message()
        {
           var waiter = GridNode.NewDebugWaiter(Timeout);

           secondStartMessage = new SleptWellEvent(Guid.NewGuid(), Guid.NewGuid(), SagaId);

           waiter.Expect<SagaCreatedEvent<SoftwareProgrammingSagaData>>()
                 .Create()
                 .Publish(new GotTiredEvent(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), SagaId),
                          secondStartMessage)
                 .Wait();

           LookupInstanceSagaActor<SoftwareProgrammingSaga, SoftwareProgrammingSagaData>(SagaId)
                                                       .Tell(NotifyOnPersistenceEvents.Instance);

           int count = 2;
           FishForMessage<Persisted>(m => ++count >=2);

           SagaData = LoadAggregate<SagaDataAggregate<SoftwareProgrammingSagaData>>(SagaId);
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