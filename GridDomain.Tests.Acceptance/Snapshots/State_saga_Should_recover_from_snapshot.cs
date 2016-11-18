using System;
using GridDomain.Common;
using GridDomain.EventSourcing.Adapters;
using GridDomain.Node;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Tests.Framework;
using GridDomain.Tests.Sagas.SoftwareProgrammingDomain.Events;
using GridDomain.Tests.Sagas.StateSagas;
using GridDomain.Tests.Sagas.StateSagas.SampleSaga;
using GridDomain.Tests.SampleDomain;
using GridDomain.Tools.Repositories;
using GridDomain.Tools.Repositories.AggregateRepositories;
using Newtonsoft.Json;
using NUnit.Framework;

namespace GridDomain.Tests.Acceptance.Snapshots
{
    [TestFixture]
    class State_saga_Should_recover_from_snapshot : SoftwareProgrammingStateSagaTest
    {
        private SoftwareProgrammingSagaState _sagaState;
        private SoftwareProgrammingSagaState _restoredState;


        public State_saga_Should_recover_from_snapshot(): base(false) { }


        protected override IContainerConfiguration CreateConfiguration()
        {
            return new CustomContainerConfiguration(
                base.CreateConfiguration(),
                SagaConfiguration.State<SoftwareProgrammingSaga,
                                             SoftwareProgrammingSagaState,
                                             SoftwareProgrammingSagaFactory,
                                             GotTiredEvent>
                                             (SoftwareProgrammingSaga.Descriptor,
                                              () => new SnapshotsSaveAfterEachMessagePolicy(),
                                              SoftwareProgrammingSagaState.FromSnapshot));
        }

        [OneTimeSetUp]
        public void Test()
        {
            _sagaState = new SoftwareProgrammingSagaState(Guid.NewGuid(), SoftwareProgrammingSaga.States.MakingCoffee);
            _sagaState.RememberPerson(Guid.NewGuid());
            _sagaState.RememberBadCoffeMachine(Guid.NewGuid());
            _sagaState.ClearEvents();

            var repo = new AggregateSnapshotRepository(AkkaConf.Persistence.JournalConnectionString, GridNode.AggregateFromSnapshotsFactory);
            repo.Add(_sagaState);

            _restoredState = LoadSagaState<SoftwareProgrammingSaga,SoftwareProgrammingSagaState>(_sagaState.Id);
        }

        [Test]
        public void PersonId_should_be_equal()
        {
            Assert.AreEqual(_sagaState.PersonId, _restoredState.PersonId);
        }

        [Test]
        public void CoffeMachineId_should_be_equal()
        {
            Assert.AreEqual(_sagaState.CoffeMachineId, _restoredState.CoffeMachineId);
        }

        [Test]
        public void State_restored_from_snapshot_should_not_have_uncommited_events()
        {
            CollectionAssert.IsEmpty(_restoredState.GetEvents());
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