using System;
using System.Threading.Tasks;
using GridDomain.CQRS.Messaging;
using GridDomain.EventSourcing.Sagas;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Tests.Framework;
using GridDomain.Tests.Sagas.SoftwareProgrammingDomain.Commands;
using GridDomain.Tests.Sagas.SoftwareProgrammingDomain.Events;
using GridDomain.Tests.Sagas.StateSagas.SampleSaga;
using Microsoft.Practices.Unity;
using NUnit.Framework;

namespace GridDomain.Tests.Sagas.StateSagas
{
    [TestFixture]
    public class SagaStart_with_predefined_id_messaging : SoftwareProgrammingStateSagaTest
    {
        [Test]
        public async Task When_dispatch_command_than_command_should_have_right_sagaId()
        {
            var sagaStartEvent = new GotTiredEvent(Guid.NewGuid()).CloneWithSaga(Guid.NewGuid());

            var expectedCommand = ( await GridNode.NewDebugWaiter()
                                                  .Expect<MakeCoffeCommand>()
                                                  .Create()
                                                  .Publish(sagaStartEvent))
                               .Message<MakeCoffeCommand>();

            Assert.AreEqual(sagaStartEvent.SagaId, expectedCommand.SagaId);
        }

        [Test]
        public async Task When_raise_saga_than_saga_event_should_have_right_sagaId()
        {
            var sagaStartEvent = new GotTiredEvent(Guid.NewGuid()).CloneWithSaga(Guid.NewGuid());

            var expectedEvent = (await GridNode.NewDebugWaiter()
                                                  .Expect<SagaCreatedEvent<SoftwareProgrammingSaga.States>>()
                                                  .Create()
                                                  .Publish(sagaStartEvent))
                               .Message<SagaCreatedEvent<SoftwareProgrammingSaga.States>>();

            Assert.AreEqual(sagaStartEvent.SagaId, expectedEvent.SagaId);
        }

        protected override TimeSpan Timeout => TimeSpan.FromSeconds(5);
    }
}