using System;
using GridDomain.EventSourcing.Adapters;
using GridDomain.Node;
using GridDomain.Tests.Framework;
using GridDomain.Tests.Sagas.StateSagas;
using GridDomain.Tests.Sagas.StateSagas.SampleSaga;
using Newtonsoft.Json;
using NUnit.Framework;
using SoftwareProgrammingSaga = GridDomain.Tests.Sagas.StateSagas.SampleSaga.SoftwareProgrammingSaga;

namespace GridDomain.Tests.Serialization
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

            var data = DomainEventSerialization.Serialize(_sagaState);
            _restoredState = DomainEventSerialization.Deserialize<SoftwareProgrammingSagaState>(data);
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