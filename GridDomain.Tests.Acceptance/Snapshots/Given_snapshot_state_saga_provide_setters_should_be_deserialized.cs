using System;
using GridDomain.EventSourcing.Adapters;
using GridDomain.Tests.Sagas.StateSagas;
using GridDomain.Tests.Sagas.StateSagas.SampleSaga;
using Newtonsoft.Json;
using NUnit.Framework;

namespace GridDomain.Tests.Acceptance.Snapshots
{
    [TestFixture]
    class Given_snapshot_state_saga_provide_setters_should_be_deserialized : ProgrammingSoftwareStateSagaTest
    {
        private SoftwareProgrammingSagaState _sagaState;
        private SoftwareProgrammingSagaState _restoredState;
        public Given_snapshot_state_saga_provide_setters_should_be_deserialized() : base(true) { }

        [OneTimeSetUp]
        public void Test()
        {
            _sagaState = new SoftwareProgrammingSagaState(Guid.NewGuid(), SoftwareProgrammingSaga.States.MakingCoffe);
            _sagaState.RememberPerson(Guid.NewGuid());

            var data = JsonConvert.SerializeObject(_sagaState, DomainEventSerialization.GetDefaultSettings());
            _restoredState = JsonConvert.DeserializeObject<SoftwareProgrammingSagaState>(data, DomainEventSerialization.GetDefaultSettings());
        }

        [Test]
        public void PersonId_should_be_equal()
        {
            Assert.AreEqual(_sagaState.PersonId, _restoredState.PersonId);
        }

        [Test]
        public void State_should_be_equal()
        {
            Assert.AreEqual(_sagaState.Id, _restoredState.Id);
        }
    }
}