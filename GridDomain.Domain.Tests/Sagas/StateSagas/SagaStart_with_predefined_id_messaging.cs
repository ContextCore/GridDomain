using System;
using GridDomain.CQRS.Messaging;
using GridDomain.EventSourcing.Sagas;
using GridDomain.Tests.Sagas.SoftwareProgrammingDomain.Commands;
using GridDomain.Tests.Sagas.SoftwareProgrammingDomain.Events;
using GridDomain.Tests.Sagas.StateSagas.SampleSaga;
using Microsoft.Practices.Unity;
using NUnit.Framework;

namespace GridDomain.Tests.Sagas.StateSagas
{
    [TestFixture]
    public class SagaStart_with_predefined_id_messaging : SoftwareProgramming_StateSaga_Test
    {
        [Test]
        public void When_dispatch_command_than_command_should_have_right_sagaId()
        {
            var publisher = GridNode.Container.Resolve<IPublisher>();
            var sagaId = Guid.NewGuid();

            var sourceId = Guid.NewGuid();
            publisher.Publish(new GotTiredEvent(sourceId).CloneWithSaga(sagaId));
            var expectedCommand = (MakeCoffeCommand)WaitFor<MakeCoffeCommand>().Message;


            Assert.AreEqual(sagaId, expectedCommand.SagaId);
        }

        [Test]
        public void When_raise_saga_than_saga_event_should_have_right_sagaId()
        {
            var publisher = GridNode.Container.Resolve<IPublisher>();
            var sagaId = Guid.NewGuid();

            var sourceId = Guid.NewGuid();
            publisher.Publish(new GotTiredEvent(sourceId).CloneWithSaga(sagaId));
            var expectedEvent =
                (SagaCreatedEvent<SoftwareProgrammingSaga.States>)
                    WaitFor<SagaCreatedEvent<SoftwareProgrammingSaga.States>>().Message;


            Assert.AreEqual(sagaId, expectedEvent.SagaId);
        }

        protected override TimeSpan Timeout => TimeSpan.FromSeconds(5);
    }
}