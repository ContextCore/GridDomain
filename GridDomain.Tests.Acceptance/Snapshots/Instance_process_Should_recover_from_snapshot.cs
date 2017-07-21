using System;
using System.Threading.Tasks;
using GridDomain.EventSourcing;
using GridDomain.ProcessManagers.State;
using GridDomain.Tests.Unit;
using GridDomain.Tests.Unit.ProcessManagers;
using GridDomain.Tests.Unit.ProcessManagers.SoftwareProgrammingDomain;
using GridDomain.Tools.Repositories.AggregateRepositories;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Acceptance.Snapshots
{
    public class Instance_process_Should_recover_from_snapshot : NodeTestKit
    {
        public Instance_process_Should_recover_from_snapshot(ITestOutputHelper helper)
            : base(helper, new SoftwareProgrammingProcessManagerFixture().UseSqlPersistence()) {}

        [Fact]
        public async Task Test()
        {
            var process  = new SoftwareProgrammingProcess();
            var state = new SoftwareProgrammingState(Guid.NewGuid(), process.Coding.Name, Guid.NewGuid(), Guid.NewGuid());

            var processStateAggregate = new ProcessStateAggregate<SoftwareProgrammingState>(state);
            processStateAggregate.ReceiveMessage(state, new object());
            processStateAggregate.PersistAll();

            var repo = new AggregateSnapshotRepository(AkkaConfig.Persistence.JournalConnectionString,
                                                       new AggregateFactory());
            await repo.Add(processStateAggregate);

            var restoredState = await this.LoadProcessByActor<SoftwareProgrammingState>(processStateAggregate.Id);
            //CoffeMachineId_should_be_equal()
            Assert.Equal(processStateAggregate.State.CoffeeMachineId,  restoredState.CoffeeMachineId);
            // State_should_be_equal()
            Assert.Equal(processStateAggregate.State.CurrentStateName, restoredState.CurrentStateName);
        }
    }
}