using System;
using System.Threading.Tasks;
using GridDomain.Common;
using GridDomain.EventSourcing;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.ProcessManagers.State;
using GridDomain.Tests.Common;
using GridDomain.Tests.Unit.ProcessManagers;
using GridDomain.Tests.Unit.ProcessManagers.SoftwareProgrammingDomain;
using GridDomain.Tests.Unit.ProcessManagers.SoftwareProgrammingDomain.Events;
using GridDomain.Tools.Repositories.AggregateRepositories;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Acceptance.Snapshots
{
    public class InstanceProcessShouldNotSaveSnapshotsOnMessageProcessByDefault : SoftwareProgrammingProcessTest
    {
        public InstanceProcessShouldNotSaveSnapshotsOnMessageProcessByDefault(ITestOutputHelper helper) : base(helper) {}

        [Fact]
        public async Task Given_default_policy()
        {
            var startEvent = new GotTiredEvent(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), Guid.NewGuid().ToString());

            var res1 = await Node.PrepareForProcessManager(startEvent)
                                 .Expect<ProcessManagerCreated<SoftwareProgrammingState>>()
                                 .Create();
            
            var res = await
                Node.NewTestWaiter()
                    .Expect<ProcessManagerCreated<SoftwareProgrammingState>>()
                    .Create()
                    .SendToProcessManagers(startEvent);

            var processId = res.Message<ProcessManagerCreated<SoftwareProgrammingState>>().SourceId;

            var continueEvent = new CoffeMakeFailedEvent(Guid.NewGuid().ToString(), startEvent.PersonId, BusinessDateTime.UtcNow, processId);

            await Node.NewTestWaiter()
                    .Expect<ProcessReceivedMessage<SoftwareProgrammingState>>()
                    .Create()
                    .SendToProcessManagers(continueEvent);

            //saving snapshot
            await Task.Delay(200);

            var snapshots =
                await
                    new AggregateSnapshotRepository(AutoTestNodeDbConfiguration.Default.JournalConnectionString,
                                                    new AggregateFactory(),
                                                    new AggregateFactory()
                                                    ).Load<ProcessStateAggregate<SoftwareProgrammingState>>(
                                                                                                                                              startEvent.ProcessId);
            //Snapshot_should_be_saved_one_time()
            Assert.Empty(snapshots);
        }
    }
}