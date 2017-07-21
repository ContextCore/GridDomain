using System;
using System.Linq;
using System.Threading.Tasks;
using GridDomain.Common;
using GridDomain.EventSourcing;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Processes.State;
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
    public class Process_Actor_Should_save_snapshots_with_max_frequency_according_to_policy : NodeTestKit
    {
        public Process_Actor_Should_save_snapshots_with_max_frequency_according_to_policy(ITestOutputHelper output)
            : base(
                   output,
                   new SoftwareProgrammingProcessManagerFixture().UseSqlPersistence().InitSnapshots(2,TimeSpan.FromSeconds(10)).IgnoreCommands()) {}

        [Fact]
        public async Task Given_default_policy()
        {
            var startEvent = new GotTiredEvent(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

            var res = await Node.NewDebugWaiter()
                                .Expect<ProcessManagerCreated<SoftwareProgrammingState>>()
                                .Create()
                                .SendToProcessManagers(startEvent);

            var processId = res.Message<ProcessManagerCreated<SoftwareProgrammingState>>().SourceId;

            var continueEvent = new CoffeMakeFailedEvent(processId, startEvent.PersonId, BusinessDateTime.UtcNow);

            //send text event
            await Node.NewDebugWaiter()
                      .Expect<ProcessReceivedMessage<SoftwareProgrammingState>>()
                      .Create()
                      .SendToProcessManagers(continueEvent, processId);


            var snapshots = await new AggregateSnapshotRepository(AkkaConfig.Persistence.JournalConnectionString,
                                                                  new AggregateFactory()).Load<ProcessStateAggregate<SoftwareProgrammingState>>(processId);

            //Snapshot_should_be_saved_one_time
            Assert.Equal(1, snapshots.Length);
            //Restored_process_state_should_have_correct_ids
            Assert.True(snapshots.All(s => s.Aggregate.Id == processId));
            //Snapshot_should_have_parameters_from_first_event = created event
            Assert.Equal(nameof(SoftwareProgrammingProcess.Coding), snapshots.First().Aggregate.State.CurrentStateName);
            //All_snapshots_should_not_have_uncommited_events
            Assert.Empty(snapshots.SelectMany(s => s.Aggregate.GetEvents()));
        }
    }
}