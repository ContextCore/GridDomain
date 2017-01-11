using System;
using System.Threading.Tasks;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Tests.Framework;
using GridDomain.Tests.Unit.Sagas.SoftwareProgrammingDomain.Commands;
using GridDomain.Tests.Unit.Sagas.SoftwareProgrammingDomain.Events;
using NUnit.Framework;

namespace GridDomain.Tests.Unit.Sagas.InstanceSagas
{
    [TestFixture]
    public class Saga_produced_events_and_commands_has_sagaId : SoftwareProgrammingInstanceSagaTest
    {
        public Saga_produced_events_and_commands_has_sagaId() : base(true)
        {

        }

        public Saga_produced_events_and_commands_has_sagaId(bool inMemory) : base(inMemory)
        {

        }

        [Test]
        public async Task When_dispatch_command_than_command_should_have_right_sagaId()
        {
            var sagaId = Guid.NewGuid();
            var sourceId = Guid.NewGuid();
            var domainEvent = new GotTiredEvent(sourceId).CloneWithSaga(sagaId);
            var expectedCommand =
                             (await GridNode.NewDebugWaiter(TimeSpan.FromHours(10))
                                            .Expect<MakeCoffeCommand>()
                                            .Create()
                                            .Publish(domainEvent)).Message<MakeCoffeCommand>();

            Assert.AreEqual(sagaId, expectedCommand.SagaId);
        }

        [Test]
        public async Task When_raise_saga_than_saga_created_event_should_have_right_sagaId()
        {
            var sagaId = Guid.NewGuid();
            var sourceId = Guid.NewGuid();
            var domainEvent = new GotTiredEvent(sourceId,Guid.NewGuid(),Guid.NewGuid(),sagaId);

            var expectedCreatedEvent =
                            (await GridNode.NewDebugWaiter()
                                           .Expect<SagaCreatedEvent<SoftwareProgrammingSagaData>>()
                                           .Create()
                                           .Publish(domainEvent)).Message<SagaCreatedEvent<SoftwareProgrammingSagaData>>();

            Assert.AreEqual(sagaId, expectedCreatedEvent.SagaId);
        }

        protected override TimeSpan Timeout => TimeSpan.FromSeconds(5);
    }
}