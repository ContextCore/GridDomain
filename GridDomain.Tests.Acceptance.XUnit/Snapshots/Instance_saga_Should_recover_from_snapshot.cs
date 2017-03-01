using System;
using System.Threading.Tasks;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Tests.Framework;
using GridDomain.Tests.XUnit;
using GridDomain.Tests.XUnit.Sagas;
using GridDomain.Tests.XUnit.Sagas.SoftwareProgrammingDomain;
using GridDomain.Tools.Repositories.AggregateRepositories;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Acceptance.XUnit.Snapshots
{
    public class Instance_saga_Should_recover_from_snapshot : NodeTestKit
    {
        public Instance_saga_Should_recover_from_snapshot(ITestOutputHelper helper)
            : base(helper, new SoftwareProgrammingSagaFixture {InMemory = false}) {}

        [Fact]
        public async Task Test()
        {
            var saga = new SoftwareProgrammingSaga();
            var state = new SoftwareProgrammingSagaData(Guid.NewGuid(), saga.Coding.Name, Guid.NewGuid(), Guid.NewGuid());

            var sagaState = new SagaStateAggregate<SoftwareProgrammingSagaData>(state);
            sagaState.RememberEvent(saga.CoffeReady, state, new object());
            sagaState.ClearEvents();

            var repo = new AggregateSnapshotRepository(AkkaConfig.Persistence.JournalConnectionString,
                                                       Node.AggregateFromSnapshotsFactory);
            await repo.Add(sagaState);

            var restoredState = await this.LoadSaga<SoftwareProgrammingSaga, SoftwareProgrammingSagaData>(sagaState.Id);
            //CoffeMachineId_should_be_equal()
            Assert.Equal(sagaState.Data.CoffeeMachineId, restoredState.Data.CoffeeMachineId);
            //State_restored_from_snapshot_should_not_have_uncommited_events()
            Assert.Empty(restoredState.GetEvents());
            // State_should_be_equal()
            Assert.Equal(sagaState.Data.CurrentStateName, restoredState.Data.CurrentStateName);
        }
    }
}