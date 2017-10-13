using System;
using System.Linq;
using System.Threading.Tasks;
using GridDomain.Common;
using GridDomain.EventSourcing;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.ProcessManagers.State;
using GridDomain.Tests.Acceptance.EventsUpgrade;
using GridDomain.Tests.Common;
using GridDomain.Tests.Unit;
using GridDomain.Tests.Unit.ProcessManagers;
using GridDomain.Tests.Unit.ProcessManagers.SoftwareProgrammingDomain;
using GridDomain.Tests.Unit.ProcessManagers.SoftwareProgrammingDomain.Events;
using GridDomain.Tools.Repositories.AggregateRepositories;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Acceptance.Snapshots
{
    public class Process_actor_Should_delete_snapshots_according_to_policy_on_shutdown : NodeTestKit
    {
        public Process_actor_Should_delete_snapshots_according_to_policy_on_shutdown(ITestOutputHelper output)
            : base(new SoftwareProgrammingProcessManagerFixture(output).UseSqlPersistence()
                                                              .InitSnapshots(2)
                                                              .IgnorePipeCommands()) { }

        [Fact]
        public async Task Given_save_on_each_message_policy_and_keep_2_snapshots()
        {
            var startEvent = new GotTiredEvent(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

            var res = await Node.NewDebugWaiter()
                                .Expect<ProcessManagerCreated<SoftwareProgrammingState>>()
                                .Create()
                                .SendToProcessManagers(startEvent);

            var processId = res.Message<ProcessManagerCreated<SoftwareProgrammingState>>().SourceId;

            var continueEventA = new CoffeMakeFailedEvent(Guid.NewGuid(),
                                                         startEvent.PersonId,
                                                         BusinessDateTime.UtcNow,
                                                         processId);

            await Node.SendToProcessManagers(continueEventA);

            await Node.KillProcessManager<SoftwareProgrammingProcess, SoftwareProgrammingState>(processId);

            var snapshots = await new AggregateSnapshotRepository(AutoTestNodeDbConfiguration.Default.JournalConnectionString, new AggregateFactory())
                                                                .Load<ProcessStateAggregate<SoftwareProgrammingState>>(processId);
            //Only_two_Snapshots_should_left()
            Assert.Equal(2, snapshots.Length);
            // Restored_aggregates_should_have_same_ids()
            Assert.True(snapshots.All(s => s.Aggregate.Id == processId));

            // First_Snapshots_should_have_coding_state_from_first_event()
            Assert.Equal(nameof(SoftwareProgrammingProcess.MakingCoffee),
                snapshots.First()
                         .Aggregate.State.CurrentStateName);

            //Last_Snapshots_should_have_coding_state_from_last_event()
            Assert.Equal(nameof(SoftwareProgrammingProcess.Sleeping),
                snapshots.Last()
                         .Aggregate.State.CurrentStateName);

            //All_snapshots_should_not_have_uncommited_events()
            Assert.Empty(snapshots.SelectMany(s => s.Aggregate.GetEvents()));
        }
    }
}