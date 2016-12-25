using System;
using GridDomain.EventSourcing.Adapters;
using GridDomain.Tests.Framework;
using GridDomain.Tests.Unit.Sagas.StateSagas;
using GridDomain.Tests.Unit.Sagas.StateSagas.SampleSaga;
using NUnit.Framework;
using SoftwareProgrammingSaga = GridDomain.Tests.Unit.Sagas.StateSagas.SampleSaga.SoftwareProgrammingSaga;

namespace GridDomain.Tests.Unit.Serialization
{
    [TestFixture]
    class State_sagas_should_be_serialize_and_deserialize : SoftwareProgrammingStateSagaTest
    {
        private SoftwareProgrammingSagaState _sagaState;
        private SoftwareProgrammingSagaState _restoredState;
        public State_sagas_should_be_serialize_and_deserialize() : base(true) { }

        [OneTimeSetUp]
        public void Test()
        {
            _sagaState = new SoftwareProgrammingSagaState(Guid.NewGuid(), SoftwareProgrammingSaga.States.MakingCoffee);
            _sagaState.RememberPerson(Guid.NewGuid());
            _sagaState.ClearEvents();

            var data = DomainSerializer.Serialize(_sagaState);
            _restoredState = DomainSerializer.Deserialize<SoftwareProgrammingSagaState>(data);
        }

        [Test]
        public void PersonId_should_be_equal()
        {
            Assert.AreEqual(_sagaState.PersonId, _restoredState.PersonId);
        }

        [Test]
        public void Id_should_be_equal()
        {
            Assert.AreEqual(_sagaState.Id, _restoredState.Id);
        }

        [Test]
        public void State_should_be_equal()
        {
            Assert.AreEqual(_sagaState.MachineState, _restoredState.MachineState);
        }
    }
}