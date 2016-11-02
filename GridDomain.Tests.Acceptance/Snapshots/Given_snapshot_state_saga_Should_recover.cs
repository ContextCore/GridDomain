using System;
using GridDomain.EventSourcing.Adapters;
using GridDomain.Node;
using GridDomain.Tests.Sagas.StateSagas;
using GridDomain.Tests.Sagas.StateSagas.SampleSaga;
using GridDomain.Tests.SampleDomain;
using GridDomain.Tools.Repositories;
using Newtonsoft.Json;
using NUnit.Framework;

namespace GridDomain.Tests.Acceptance.Snapshots
{
    [TestFixture]
    class Given_snapshot_state_saga_Should_recover : ProgrammingSoftwareStateSagaTest
    {
        private SoftwareProgrammingSagaState _sagaState;
        private SoftwareProgrammingSagaState _restoredState;
        public Given_snapshot_state_saga_Should_recover(): base(false) { }

        [OneTimeSetUp]
        public void Test()
        {
            _sagaState = new SoftwareProgrammingSagaState(Guid.NewGuid(), SoftwareProgrammingSaga.States.MakingCoffe);
            _sagaState.RememberPerson(Guid.NewGuid());

            var repo = new AggregateSnapshotRepository(AkkaConf.Persistence.JournalConnectionString);
            repo.Add(_sagaState);

            _restoredState = LoadSagaState<SoftwareProgrammingSaga,SoftwareProgrammingSagaState>(_sagaState.Id);
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