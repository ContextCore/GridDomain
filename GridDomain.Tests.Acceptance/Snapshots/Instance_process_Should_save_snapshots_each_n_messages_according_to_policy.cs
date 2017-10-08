using System;
using System.Linq;
using System.Threading.Tasks;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.EventSourcing;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.ProcessManagers.State;
using GridDomain.Tests.Acceptance.EventsUpgrade;
using GridDomain.Tests.Common;
using GridDomain.Tests.Unit;
using GridDomain.Tests.Unit.ProcessManagers;
using GridDomain.Tests.Unit.ProcessManagers.SoftwareProgrammingDomain;
using GridDomain.Tests.Unit.ProcessManagers.SoftwareProgrammingDomain.Commands;
using GridDomain.Tests.Unit.ProcessManagers.SoftwareProgrammingDomain.Events;
using GridDomain.Tools.Repositories.AggregateRepositories;
using Serilog.Events;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Acceptance.Snapshots
{
    public class Instance_process_Should_save_snapshots_each_n_messages_according_to_policy : NodeTestKit
    {
        public Instance_process_Should_save_snapshots_each_n_messages_according_to_policy(ITestOutputHelper output)
            : base(output,
                new SoftwareProgrammingProcessManagerFixture().UseSqlPersistence()
                                                              .InitSnapshots(5, TimeSpan.FromMilliseconds(1), 2)
                                                              .IgnoreCommands())
        {
            Fixture.LogLevel = LogEventLevel.Verbose;
        }

        [Fact]
        public async Task Given_default_policy()
        {
            var startEvent = new GotTiredEvent(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

            var res = await Node.NewDebugWaiter()
                                .Expect<ProcessManagerCreated<SoftwareProgrammingState>>()
                                .Create()
                                .SendToProcessManagers(startEvent);

            var processId = res.Message<ProcessManagerCreated<SoftwareProgrammingState>>()
                               .SourceId;

            var continueEvent = new CoffeMakeFailedEvent(processId, startEvent.PersonId, BusinessDateTime.UtcNow, processId);

            await Node.NewDebugWaiter()
                      .Expect<ProcessReceivedMessage<SoftwareProgrammingState>>()
                      .Create()
                      .SendToProcessManagers(continueEvent);

            var continueEventB =
                new Fault<GoSleepCommand>(new GoSleepCommand(startEvent.PersonId, startEvent.LovelySofaId),
                                          new Exception(),
                                          typeof(object),
                                          processId,
                                          BusinessDateTime.Now);

            await Node.NewDebugWaiter()
                      .Expect<ProcessReceivedMessage<SoftwareProgrammingState>>()
                      .Create()
                      .SendToProcessManagers(continueEventB);

            await Node.KillProcessManager<SoftwareProgrammingProcess,SoftwareProgrammingState>(continueEventB.ProcessId);

            var snapshots = await new AggregateSnapshotRepository(AutoTestNodeDbConfiguration.Default.JournalConnectionString,
                                                                  new AggregateFactory()).Load<ProcessStateAggregate<SoftwareProgrammingState>>(processId);

            //saving on each message, maximum on each command
            //Snapshots_should_be_saved_two_times
            //4 events in total, two saves of snapshots due to policy saves on each two events
            Assert.Equal(2, snapshots.Length);
            //First_snapshot_should_have_state_from_first_event
            Assert.Equal(nameof(SoftwareProgrammingProcess.MakingCoffee),
                         snapshots.First()
                                  .Aggregate.State.CurrentStateName);
            //Last_snapshot_should_have_parameters_from_last_command()
            Assert.Equal(nameof(SoftwareProgrammingProcess.Coding),
                         snapshots.Last()
                                  .Aggregate.State.CurrentStateName);

            //Restored_process_state_should_have_correct_ids
            Assert.True(snapshots.All(s => s.Aggregate.Id == processId));
            //All_snapshots_should_not_have_uncommited_events()
            Assert.Empty(snapshots.SelectMany(s => s.Aggregate.GetEvents()));
        }
    }
}