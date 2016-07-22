using System;
using GridDomain.CQRS.Messaging;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Tests.Sagas.InstanceSagas.Commands;
using GridDomain.Tests.Sagas.InstanceSagas.Events;
using Microsoft.Practices.Unity;
using NUnit.Framework;

namespace GridDomain.Tests.Sagas.InstanceSagas
{
    [TestFixture]
    public class Saga_produced_events_and_commands_has_sagaId : ProgrammingSoftwareSagaTest
    {
        public Saga_produced_events_and_commands_has_sagaId():base(true)
        {

        }

        public Saga_produced_events_and_commands_has_sagaId(bool inMemory) : base(inMemory)
        {

        }

        [Test]
        public void When_dispatch_command_than_command_should_have_right_sagaId()
        {
            var publisher = GridNode.Container.Resolve<IPublisher>();
            var sagaId = Guid.NewGuid();

            var sourceId = Guid.NewGuid();
            publisher.Publish(new GotTiredDomainEvent(sourceId).CloneWithSaga(sagaId));
            var expectedCommand = (MakeCoffeCommand)WaitFor<MakeCoffeCommand>().Message;


            Assert.AreEqual(sagaId, expectedCommand.SagaId);
        }

        [Test]
        public void When_raise_saga_than_saga_created_event_should_have_right_sagaId()
        {
            var publisher = GridNode.Container.Resolve<IPublisher>();
            var sagaId = Guid.NewGuid();

            var sourceId = Guid.NewGuid();
            publisher.Publish(new GotTiredDomainEvent(sourceId).CloneWithSaga(sagaId));

            var expectedCreatedEvent =
                (SagaCreatedEvent<SoftwareProgrammingSagaData>)
                    WaitFor<SagaCreatedEvent<SoftwareProgrammingSagaData>>().Message;


            Assert.AreEqual(sagaId, expectedCreatedEvent.SagaId);
        }

        [Test]
        public void When_raise_saga_than_saga_transitioned_event_should_have_right_sagaId()
        {
            var publisher = GridNode.Container.Resolve<IPublisher>();
            var sagaId = Guid.NewGuid();

            var sourceId = Guid.NewGuid();
            publisher.Publish(new GotTiredDomainEvent(sourceId).CloneWithSaga(sagaId));

            var expectedTransitionedEvent =
               (SagaTransitionEvent<SoftwareProgrammingSagaData>)
                   WaitFor<SagaTransitionEvent<SoftwareProgrammingSagaData>>().Message;


            Assert.AreEqual(sagaId, expectedTransitionedEvent.SagaId);
        }

        protected override TimeSpan Timeout => TimeSpan.FromSeconds(5);
    }
}