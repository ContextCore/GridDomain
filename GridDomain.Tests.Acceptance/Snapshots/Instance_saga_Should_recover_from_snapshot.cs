using System;
using System.Threading.Tasks;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Tests.Framework;
using GridDomain.Tests.Unit.Sagas.InstanceSagas;
using GridDomain.Tools.Repositories;
using GridDomain.Tools.Repositories.AggregateRepositories;
using Newtonsoft.Json;
using NUnit.Framework;

namespace GridDomain.Tests.Acceptance.Snapshots
{
    [TestFixture]
    class Instance_saga_Should_recover_from_snapshot : SoftwareProgrammingInstanceSagaTest
    {

        private SagaStateAggregate<SoftwareProgrammingSagaData> _sagaState;
        private SagaStateAggregate<SoftwareProgrammingSagaData> _restoredState;
        public Instance_saga_Should_recover_from_snapshot() : base(false) { }

        protected override TimeSpan DefaultTimeout { get; } = TimeSpan.FromSeconds(3);

        [OneTimeSetUp]
        public async Task Test()
        {
            var saga = new SoftwareProgrammingSaga();
            var state = new SoftwareProgrammingSagaData(Guid.NewGuid(),saga.Coding.Name, Guid.NewGuid(), Guid.NewGuid());

            _sagaState = new SagaStateAggregate<SoftwareProgrammingSagaData>(state);
            _sagaState.RememberEvent(saga.CoffeReady, state, new object());
            _sagaState.ClearEvents();

            var repo = new AggregateSnapshotRepository(AkkaConf.Persistence.JournalConnectionString, GridNode.AggregateFromSnapshotsFactory);
            repo.Add(_sagaState);

            _restoredState = await LoadSaga<SoftwareProgrammingSaga, SoftwareProgrammingSagaData>(_sagaState.Id);
        }
         
        [Test]
        public void CoffeMachineId_should_be_equal()
        {
            Assert.AreEqual(_sagaState.Data.CoffeeMachineId, _restoredState.Data.CoffeeMachineId);
        }

        [Test]
        public void State_restored_from_snapshot_should_not_have_uncommited_events()
        {
            CollectionAssert.IsEmpty(_restoredState.GetEvents());
        }

        [Test]
        public void State_should_be_equal()
        {
            Assert.AreEqual(_sagaState.Data.CurrentStateName, _restoredState.Data.CurrentStateName);
        }
    }
}