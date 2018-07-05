using System;
using System.Linq;
using System.Threading.Tasks;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.EventSourcing;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.ProcessManagers.State;
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
    public class Process_Should_save_snapshots_each_n_messages_according_to_policy : NodeTestKit
    {
        protected Process_Should_save_snapshots_each_n_messages_according_to_policy(NodeTestFixture fixture) : base(fixture) { }

        public Process_Should_save_snapshots_each_n_messages_according_to_policy(ITestOutputHelper output)
            : this(ConfigureFixture(new SoftwareProgrammingProcessManagerFixture(output))) { }

        protected static NodeTestFixture ConfigureFixture(SoftwareProgrammingProcessManagerFixture softwareProgrammingProcessManagerFixture)
        {
            return softwareProgrammingProcessManagerFixture
                   .LogLevel(LogEventLevel.Debug)
                   .UseSqlPersistence()
                   .InitSnapshots(5, TimeSpan.FromMilliseconds(1), 2)
                   .IgnorePipeCommands();
        }

        [Fact]
        public async Task Given_default_policy()
        {
            var startEvent = new GotTiredEvent(Guid.NewGuid()
                                                   .ToString(),
                                               Guid.NewGuid()
                                                   .ToString(),
                                               Guid.NewGuid()
                                                   .ToString());

            var res = await Node.PrepareForProcessManager(startEvent)
                                .Expect<ProcessManagerCreated<SoftwareProgrammingState>>()
                                .Send();

            var processId = res.Message<ProcessManagerCreated<SoftwareProgrammingState>>()
                               .SourceId;

            var continueEvent = new CoffeMakeFailedEvent(processId, startEvent.PersonId, BusinessDateTime.UtcNow, processId);

            await Node.PrepareForProcessManager(continueEvent)
                      .Expect<ProcessReceivedMessage<SoftwareProgrammingState>>()
                      .Send();

            var goSleepCommand = new GoSleepCommand(startEvent.PersonId, startEvent.LovelySofaId);

            var continueEventB =
                new Fault<GoSleepCommand>("fault_sleep",
                                          goSleepCommand,
                                          new Exception(),
                                          typeof(object),
                                          processId,
                                          BusinessDateTime.Now);

            await Node.PrepareForProcessManager(continueEventB, MessageMetadata.New(goSleepCommand.Id, goSleepCommand.Id))
                      .Expect<ProcessReceivedMessage<SoftwareProgrammingState>>()
                      .Send();

            await Node.KillProcessManager<SoftwareProgrammingProcess, SoftwareProgrammingState>(continueEvent.ProcessId);

            Version<ProcessStateAggregate<SoftwareProgrammingState>>[] snapshots = null;


            AwaitAssert(() =>
                        {
                            snapshots = new AggregateSnapshotRepository(AutoTestNodeDbConfiguration.Default.JournalConnectionString,
                                                                        AggregateFactory.Default,
                                                                        AggregateFactory.Default)
                                        .Load<ProcessStateAggregate<SoftwareProgrammingState>>(processId)
                                        .Result;

                            //saving on each message, maximum on each command
                            //Snapshots_should_be_saved_two_times
                            //4 events in total, two saves of snapshots due to policy saves on each two events
                            //1 event and 3
                            Assert.Equal(3, snapshots.Length);

                            Assert.True(snapshots.All(s => s.Payload.Id == processId));
                            //All_snapshots_should_not_have_uncommited_events()
                            Assert.Empty(snapshots.SelectMany(s => s.Payload.GetEvents()));
                        },
                        TimeSpan.FromSeconds(10),
                        TimeSpan.FromSeconds(1));
        }
    }
}