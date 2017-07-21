using System;
using System.Threading.Tasks;
using GridDomain.EventSourcing;
using GridDomain.Processes.State;
using GridDomain.Tests.Unit;
using GridDomain.Tests.Unit.Sagas;
using GridDomain.Tests.Unit.Sagas.SoftwareProgrammingDomain;
using GridDomain.Tools.Repositories.AggregateRepositories;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Acceptance.Snapshots
{
    public class Instance_saga_Should_recover_from_snapshot : NodeTestKit
    {
        public Instance_saga_Should_recover_from_snapshot(ITestOutputHelper helper)
            : base(helper, new SoftwareProgrammingSagaFixture().UseSqlPersistence()) {}

        [Fact]
        public async Task Test()
        {
            var saga  = new SoftwareProgrammingProcess();
            var state = new SoftwareProgrammingState(Guid.NewGuid(), saga.Coding.Name, Guid.NewGuid(), Guid.NewGuid());

            var sagaState = new ProcessStateAggregate<SoftwareProgrammingState>(state);
            sagaState.ReceiveMessage(state, new object());
            sagaState.PersistAll();

            var repo = new AggregateSnapshotRepository(AkkaConfig.Persistence.JournalConnectionString,
                                                       new AggregateFactory());
            await repo.Add(sagaState);

            var restoredState = await this.LoadSagaByActor<SoftwareProgrammingState>(sagaState.Id);
            //CoffeMachineId_should_be_equal()
            Assert.Equal(sagaState.State.CoffeeMachineId,  restoredState.CoffeeMachineId);
            // State_should_be_equal()
            Assert.Equal(sagaState.State.CurrentStateName, restoredState.CurrentStateName);
        }
    }
}