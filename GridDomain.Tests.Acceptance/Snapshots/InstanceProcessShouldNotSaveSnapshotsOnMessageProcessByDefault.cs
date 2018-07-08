using System;
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
using GridDomain.Tests.Unit.ProcessManagers.SoftwareProgrammingDomain.Events;
using GridDomain.Tools.Repositories.AggregateRepositories;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Acceptance.Snapshots
{
    public class InstanceProcessShouldNotSaveSnapshotsOnMessageProcessByDefault : NodeTestKit
    {
        public InstanceProcessShouldNotSaveSnapshotsOnMessageProcessByDefault(ITestOutputHelper output) : this(new SoftwareProgrammingProcessManagerFixture(output)) { }
        protected InstanceProcessShouldNotSaveSnapshotsOnMessageProcessByDefault(NodeTestFixture fixture) : base(fixture) { }

        [Fact]
        public async Task Given_default_policy()
        {
            var startEvent = new GotTiredEvent(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), Guid.NewGuid().ToString());

            var res = await
                Node.PrepareForProcessManager(startEvent)
                    .Expect<ProcessManagerCreated<SoftwareProgrammingState>>()
                    .Send();

            var processId = res.Message<ProcessManagerCreated<SoftwareProgrammingState>>().SourceId;

            var continueEvent = new CoffeMakeFailedEvent(Guid.NewGuid().ToString(), startEvent.PersonId, BusinessDateTime.UtcNow, processId);

            await Node.PrepareForProcessManager(continueEvent)
                    .Expect<ProcessReceivedMessage<SoftwareProgrammingState>>()
                    .Send();

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