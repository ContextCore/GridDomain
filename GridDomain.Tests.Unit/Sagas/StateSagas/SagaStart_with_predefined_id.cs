using System;
using System.Threading;
using GridDomain.CQRS.Messaging;
using GridDomain.Tests.Unit.Sagas.SoftwareProgrammingDomain.Events;
using GridDomain.Tests.Unit.Sagas.StateSagas.SampleSaga;
using Microsoft.Practices.Unity;
using NUnit.Framework;

namespace GridDomain.Tests.Unit.Sagas.StateSagas
{
    [TestFixture]
    public class SagaStart_with_predefined_id : SoftwareProgrammingStateSagaTest
    {
        private Guid _sagaId;
        private SoftwareProgrammingSagaState _sagaState;
        private Guid _personId;
        private Guid _coffeeMachineId;

        [OneTimeSetUp]
        public void When_remember_message()
        {
            var publisher = GridNode.Container.Resolve<IPublisher>();
            _sagaId = Guid.NewGuid();

            _personId = Guid.NewGuid();
            _coffeeMachineId = Guid.NewGuid();
            publisher.Publish(new GotTiredEvent(_personId).CloneWithSaga(_sagaId));
            publisher.Publish(new CoffeMakeFailedEvent(_coffeeMachineId, _personId).CloneWithSaga(_sagaId));

            Thread.Sleep(TimeSpan.FromSeconds(1));

            _sagaState = LoadSagaState<SoftwareProgrammingSaga, SoftwareProgrammingSagaState>(_sagaId);

        }

        [Then]
        public void Saga_state_should_have_changed_state()
        {
            Assert.AreEqual(SoftwareProgrammingSaga.States.Sleeping, _sagaState.MachineState);
        }

        [Then]
        public void Saga_state_should_remember_person_from_event()
        {
            Assert.AreEqual(_personId, _sagaState.PersonId);
        }

        [Then]
        public void Saga_state_should_remember_coffe_machine_from_event()
        {
            Assert.AreEqual(_coffeeMachineId, _sagaState.CoffeMachineId);
        }

        [Then]
        public void Saga_state_should_has_saga_Id()
        {
            Assert.AreEqual(_sagaId, _sagaState.Id);
        }

        protected override TimeSpan Timeout => TimeSpan.FromSeconds(5);        
    }
}
