using System;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Tests.Sagas.InstanceSagas;
using GridDomain.Tools.Repositories;
using Newtonsoft.Json;
using NUnit.Framework;

namespace GridDomain.Tests.Acceptance.Snapshots
{
    [TestFixture]
    class Given_snapshot_instance_saga_Should_recover : SoftwareProgrammingInstanceSagaTest
    {

        private SagaDataAggregate<SoftwareProgrammingSagaData> _sagaState;
        private SagaDataAggregate<SoftwareProgrammingSagaData> _restoredState;
        public Given_snapshot_instance_saga_Should_recover() : base(true) { }

        [OneTimeSetUp]
        public void Test()
        {
            var saga = new SoftwareProgrammingSaga();
            var state = new SoftwareProgrammingSagaData("123", Guid.NewGuid(), Guid.NewGuid());

            _sagaState = new SagaDataAggregate<SoftwareProgrammingSagaData>(Guid.NewGuid(), state);
            _sagaState.RememberEvent(saga.CoffeReady, state, new object());

            var repo = new AggregateSnapshotRepository(AkkaConf.Persistence.JournalConnectionString);
            repo.Add(_sagaState);

        //    _restoredState = Load
        }


        [Test]
        public void CoffeMachineId_should_be_equal()
        {
            Assert.AreEqual(_sagaState.Data.CoffeeMachineId, _restoredState.Data.CoffeeMachineId);
        }

        [Test]
        public void Id_should_be_equal()
        {
            Assert.AreEqual(_sagaState.Id, _restoredState.Id);
        }

        [Test]
        public void State_should_be_equal()
        {
            Assert.AreEqual(_sagaState.Data.CurrentStateName, _restoredState.Data.CurrentStateName);
        }
    }
}