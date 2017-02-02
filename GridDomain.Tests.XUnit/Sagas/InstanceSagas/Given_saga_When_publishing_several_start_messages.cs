using System;
using System.Threading.Tasks;
using Akka.Actor;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Node.Actors;
using GridDomain.Tests.Framework;
using GridDomain.Tests.XUnit.Sagas.SoftwareProgrammingDomain.Events;
using Xunit;

namespace GridDomain.Tests.XUnit.Sagas.InstanceSagas
{
    public class Given_saga_When_publishing_several_start_messages : SoftwareProgrammingInstanceSagaTest
    {
        [Fact]
        public async Task When_publishing_start_message()
        {
            var anyMessagePublisher = GridNode.NewDebugWaiter(DefaultTimeout)
                                              .Expect<SagaCreatedEvent<SoftwareProgrammingSagaData>>()
                                              .Create();
            var sagaId = Guid.NewGuid();

            await anyMessagePublisher.SendToSagas(new GotTiredEvent(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), sagaId));

            var actorRef = await LookupSagaActor<SoftwareProgrammingSaga, SoftwareProgrammingSagaData>(sagaId);
            actorRef.Tell(NotifyOnPersistenceEvents.Instance);

            var secondStartMessage = new SleptWellEvent(Guid.NewGuid(), Guid.NewGuid(), sagaId);

            await anyMessagePublisher.SendToSagas(secondStartMessage);

            FishForMessage<Persisted>(m => true);

            var sagaData = LoadAggregate<SagaStateAggregate<SoftwareProgrammingSagaData>>(sagaId);
            //Saga_reinitialized_from_last_start_message()
            Assert.Equal(secondStartMessage.SofaId, sagaData.Data.SofaId);
            //Saga_has_correct_state()
            Assert.Equal(nameof(SoftwareProgrammingSaga.Coding), sagaData.Data.CurrentStateName);
        }
    }
}