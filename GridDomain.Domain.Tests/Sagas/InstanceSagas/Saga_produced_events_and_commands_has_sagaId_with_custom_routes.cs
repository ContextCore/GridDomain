using System;
using GridDomain.CQRS.Messaging;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Tests.Sagas.SoftwareProgrammingDomain.Commands;
using GridDomain.Tests.Sagas.SoftwareProgrammingDomain.Events;
using Microsoft.Practices.Unity;
using NUnit.Framework;

namespace GridDomain.Tests.Sagas.InstanceSagas
{
    [TestFixture]
    public class Saga_produced_events_and_commands_has_sagaId_with_custom_routes :
        ProgrammingSoftwareSagaTest_with_custom_routes
    {

        public Saga_produced_events_and_commands_has_sagaId_with_custom_routes() : base(true)
        {

        }

        [Test]
        public void When_dispatch_command_than_command_should_have_right_sagaId()
        {
            var publisher = GridNode.Container.Resolve<IPublisher>();

            var gotTiredEvent = new GotTiredEvent(Guid.NewGuid());
            publisher.Publish(gotTiredEvent);
            var expectedCommand = (MakeCoffeCommand)WaitFor<MakeCoffeCommand>().Message;

            Assert.AreEqual(gotTiredEvent.PersonId, expectedCommand.SagaId);
        }

        [Test]
        public void When_raise_saga_than_saga_created_event_should_have_right_sagaId()
        {
            var publisher = GridNode.Container.Resolve<IPublisher>();

            var gotTiredEvent = new GotTiredEvent(Guid.NewGuid());
            publisher.Publish(gotTiredEvent);

            var expectedCreatedEvent =
                (SagaCreatedEvent<SoftwareProgrammingSagaData>)
                    WaitFor<SagaCreatedEvent<SoftwareProgrammingSagaData>>().Message;


            Assert.AreEqual(gotTiredEvent.PersonId, expectedCreatedEvent.SagaId);
        }

        [Test]
        public void When_raise_saga_than_saga_transitioned_event_should_have_right_sagaId()
        {
            var publisher = GridNode.Container.Resolve<IPublisher>();

            var gotTiredEvent = new GotTiredEvent(Guid.NewGuid());
            publisher.Publish(gotTiredEvent);

            var expectedTransitionedEvent =
                (SagaTransitionEvent<SoftwareProgrammingSagaData>)
                    WaitFor<SagaTransitionEvent<SoftwareProgrammingSagaData>>().Message;

            Assert.AreEqual(gotTiredEvent.PersonId, expectedTransitionedEvent.SagaId);
        }

        protected override TimeSpan Timeout => TimeSpan.FromSeconds(200);
    }
}