using System;
using System.Diagnostics;
using System.Threading;
using GridDomain.CQRS.Messaging;
using GridDomain.Tests.Sagas.SoftwareProgrammingDomain.Events;
using GridDomain.Tests.Sagas.StateSagas.SampleSaga;
using Microsoft.Practices.Unity;
using NUnit.Framework;

namespace GridDomain.Tests.Sagas.StateSagas
{
    [TestFixture]
    public class Given_SagaStart_with_predifined_id:
        SoftwareProgramming_StateSaga_Test
    {
        private Guid _sagaId;
        private SoftwareProgrammingSagaState _sagaState;
        private Guid _personId;
        //_and_state_aggregate_change_in_process
        [TestFixtureSetUp]
        public void When_start_message_has_saga_id()
        {
            _sagaId = Guid.NewGuid();
            _personId = Guid.NewGuid();

            var publisher = GridNode.Container.Resolve<IPublisher>();
            publisher.Publish(new GotTiredEvent(_personId).CloneWithSaga(_sagaId));
            Thread.Sleep(500);
            _sagaState = LoadSagaState<SoftwareProgrammingSaga,
                                       SoftwareProgrammingSagaState,
                                       GotTiredEvent>(_sagaId);
        }

        [Then]
        public void Saga_state_fills_from_message_data()
        {
            Assert.AreEqual(_personId, _sagaState.PersonId);
        }

        [Then]
        public void Saga_starts_with_id_from_start_message()
        {
            Assert.AreEqual(_sagaId, _sagaState.Id);
        }

        [Then]
        public void Saga_state_is_correctly_changed()
        {
            Assert.AreEqual(SoftwareProgrammingSaga.States.MakingCoffe, _sagaState.MachineState);

        }
    }
}