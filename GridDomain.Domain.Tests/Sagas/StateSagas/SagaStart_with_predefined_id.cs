using System;
using System.Diagnostics;
using System.Threading;
using GridDomain.CQRS.Messaging;
using GridDomain.Node;
using GridDomain.Node.Configuration.Akka;
using GridDomain.Node.Configuration.Persistence;
using GridDomain.Tests.Framework;
using GridDomain.Tests.Framework.Configuration;
using GridDomain.Tests.Sagas.StateSagas.SampleSaga;
using GridDomain.Tests.Sagas.StateSagas.SampleSaga.Events;
using Microsoft.Practices.Unity;
using NUnit.Framework;

namespace GridDomain.Tests.Sagas.StateSagas
{
    [TestFixture]
    public class SagaStart_with_predefined_id : SoftwareProgramming_StateSaga_Test
    {
        [Test]
        public void When_remember_message_than_saga_state_should_be_changed()
        {
            var publisher = GridNode.Container.Resolve<IPublisher>();
            var sagaId = Guid.NewGuid();

            var sourceId = Guid.NewGuid();
            publisher.Publish(new GotTiredEvent(sourceId).CloneWithSaga(sagaId));
            publisher.Publish(new GotMoreTiredEvent(sourceId).CloneWithSaga(sagaId));

            Thread.Sleep(Debugger.IsAttached ? TimeSpan.FromSeconds(1000) : TimeSpan.FromSeconds(1));

            var sagaState = LoadSagaState<SoftwareProgrammingSaga,
                                          SoftwareProgrammingSagaState,
                                          GotTiredEvent>(sagaId);

            Assert.AreEqual(sagaId, sagaState.Id);
            Assert.AreEqual(SoftwareProgrammingSaga.States.Sleeping, sagaState.MachineState);
            Assert.AreEqual(sagaState.SourceId, sourceId);
        }
    }
}
