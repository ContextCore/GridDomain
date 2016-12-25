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
    public class Saga_produced_events_and_commands_has_sagaId_with_custom_routes :
        ProgrammingSoftwareSagaTest_with_custom_routes
    {

        public Saga_produced_events_and_commands_has_sagaId_with_custom_routes() : base(true)
        {

        }

        [Test]
        public async Task When_dispatch_command_than_command_should_have_right_sagaId()
        {
            var gotTiredEvent = new GotTiredEvent(Guid.NewGuid());
            var expectedCommand =
                          (await GridNode.NewDebugWaiter()
                                         .Expect<MakeCoffeCommand>()
                                         .Create()
                                         .Publish(gotTiredEvent)).Message<MakeCoffeCommand>();

            Assert.AreEqual(gotTiredEvent.PersonId, expectedCommand.SagaId);
        }

        [Test]
        public async Task When_raise_saga_than_saga_created_event_should_have_right_sagaId()
        {
            var gotTiredEvent = new GotTiredEvent(Guid.NewGuid());

            var expectedCreatedEvent =
                         (await GridNode.NewDebugWaiter()
                                        .Expect<SagaCreatedEvent<SoftwareProgrammingSagaData>>()
                                        .Create()
                                        .Publish(gotTiredEvent)).Message<SagaCreatedEvent<SoftwareProgrammingSagaData>>();

            Assert.AreEqual(gotTiredEvent.PersonId, expectedCreatedEvent.SagaId);
        }

        [Test]
        public async Task When_raise_saga_than_saga_transitioned_event_should_have_right_sagaId()
        {
            var gotTiredEvent = new GotTiredEvent(Guid.NewGuid());

            var expectedTransitionedEvent =
                       (await GridNode.NewDebugWaiter()
                                      .Expect<SagaTransitionEvent<SoftwareProgrammingSagaData>>()
                                      .Create()
                                      .Publish(gotTiredEvent)).Message<SagaTransitionEvent<SoftwareProgrammingSagaData>>();

            Assert.AreEqual(gotTiredEvent.PersonId, expectedTransitionedEvent.SagaId);
        }

    }
}