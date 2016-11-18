using System;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Tests.Framework;
using GridDomain.Tests.Sagas.InstanceSagas;
using GridDomain.Tools.Repositories;
using GridDomain.Tools.Repositories.AggregateRepositories;
using Newtonsoft.Json;
using NUnit.Framework;

namespace GridDomain.Tests.Acceptance.Snapshots
{
    [TestFixture]
    class Instance_saga_Should_recover_from_snapshot : SoftwareProgrammingInstanceSagaTest
    {

        private SagaDataAggregate<SoftwareProgrammingSagaData> _sagaState;
        private SagaDataAggregate<SoftwareProgrammingSagaData> _restoredState;
        public Instance_saga_Should_recover_from_snapshot() : base(false) { }

        protected override TimeSpan Timeout { get; } = TimeSpan.FromSeconds(3);

        [OneTimeSetUp]
        public void Test()
        {
            var saga = new SoftwareProgrammingSaga();
            var state = new SoftwareProgrammingSagaData(saga.Coding.Name, Guid.NewGuid(), Guid.NewGuid());

            _sagaState = new SagaDataAggregate<SoftwareProgrammingSagaData>(Guid.NewGuid(), state);
            _sagaState.RememberEvent(saga.CoffeReady, state, new object());
            _sagaState.ClearEvents();

            var repo = new AggregateSnapshotRepository(AkkaConf.Persistence.JournalConnectionString, GridNode.AggregateFromSnapshotsFactory);
            repo.Add(_sagaState);

            _restoredState = LoadInstanceSagaState<SoftwareProgrammingSaga, SoftwareProgrammingSagaData>(_sagaState.Id);
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