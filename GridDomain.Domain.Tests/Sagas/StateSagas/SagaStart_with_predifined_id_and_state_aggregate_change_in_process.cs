using System;
using System.Diagnostics;
using System.Threading;
using GridDomain.CQRS.Messaging;
using GridDomain.Tests.Sagas.StateSagas.SampleSaga;
using GridDomain.Tests.Sagas.StateSagas.SampleSaga.Events;
using Microsoft.Practices.Unity;
using NUnit.Framework;

namespace GridDomain.Tests.Sagas.StateSagas
{
    [TestFixture]
    public class SagaStart_with_predifined_id_and_state_aggregate_change_in_process:
        SoftwareProgramming_StateSaga_Test
    {

        [Test]
        public void When_start_message_has_saga_id_Saga_starts_with_it()
        {
            var publisher = GridNode.Container.Resolve<IPublisher>();
            var sagaId = Guid.NewGuid();

            publisher.Publish(new GotTiredEvent(Guid.NewGuid()).CloneWithSaga(sagaId));

            Thread.Sleep(Debugger.IsAttached ? TimeSpan.FromSeconds(1000) : TimeSpan.FromSeconds(1));

            var sagaState = LoadSagaState<SoftwareProgrammingSaga,
                SoftwareProgrammingSagaState,
                GotTiredEvent>(sagaId);

            Assert.AreEqual(sagaId, sagaState.Id);
            Assert.AreEqual(SoftwareProgrammingSaga.States.DrinkingCoffe, sagaState.MachineState);
        }
    }
}