using System;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Persistence;
using GridDomain.Common;
using GridDomain.Configuration;
using GridDomain.EventSourcing;
using GridDomain.Node.Actors.EventSourced.Messages;
using GridDomain.Node.Actors.PersistentHub;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.ProcessManagers;
using GridDomain.ProcessManagers.State;
using GridDomain.Tests.Acceptance.EventsUpgrade;
using GridDomain.Tests.Common;
using GridDomain.Tests.Unit;
using GridDomain.Tests.Unit.ProcessManagers;
using GridDomain.Tests.Unit.ProcessManagers.SoftwareProgrammingDomain;
using GridDomain.Tests.Unit.ProcessManagers.SoftwareProgrammingDomain.Configuration;
using GridDomain.Tests.Unit.ProcessManagers.SoftwareProgrammingDomain.Events;
using GridDomain.Tools.Repositories.AggregateRepositories;
using Xunit;
using Xunit.Abstractions;
using GridDomain.Transport.Remote;

namespace GridDomain.Tests.Acceptance.Snapshots
{
    public class Process_Actor_Should_save_snapshots_with_max_frequency_according_to_policy : NodeTestKit
    {
        public Process_Actor_Should_save_snapshots_with_max_frequency_according_to_policy(ITestOutputHelper output)
            : base(new SoftwareProgrammingProcessManagerFixture(output).UseSqlPersistence()
                                                                       .InitSnapshots(2, TimeSpan.FromSeconds(10))
                                                                       .IgnorePipeCommands()) { }

       
        [Fact]
        public async Task Given_default_policy()
        {
            var startEvent = new GotTiredEvent(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

            var resTask = Node.NewDebugWaiter()
                              .Expect<ProcessReceivedMessage<SoftwareProgrammingState>>()
                              .Create()
                              .SendToProcessManagers(startEvent);

            var processId = (await resTask).Message<ProcessReceivedMessage<SoftwareProgrammingState>>().SourceId;

            var continueEvent = new CoffeMakeFailedEvent(processId, startEvent.PersonId, BusinessDateTime.UtcNow, processId);

            //to avoid racy state receiving expected message from processing GotTiredEvent 
            await Node.NewDebugWaiter()
                      .Expect<ProcessReceivedMessage<SoftwareProgrammingState>>(
                                        e => (e.Message as IMessageMetadataEnvelop)?.Message is CoffeMakeFailedEvent)
                      .Create()
                      .SendToProcessManagers(continueEvent, processId);

            this.Log.Info("Enforced additional snapshot save & delete");
            await Node.KillProcessManager<SoftwareProgrammingProcess, SoftwareProgrammingState>(processId);


            var snapshots = await new AggregateSnapshotRepository(AutoTestNodeDbConfiguration.Default.JournalConnectionString,
                                                                  new AggregateFactory()).Load<ProcessStateAggregate<SoftwareProgrammingState>>(processId);

            //Snapshot_should_be_saved_one_time
            Assert.Single(snapshots);
            //Restored_process_state_should_have_correct_ids
            Assert.True(snapshots.All(s => s.Aggregate.Id == processId));
            //Snapshot_should_have_parameters_from_first_event = created event
            Assert.Equal(nameof(SoftwareProgrammingProcess.Coding),
                         snapshots.First()
                                  .Aggregate.State.CurrentStateName);
            //All_snapshots_should_not_have_uncommited_events
            Assert.Empty(snapshots.SelectMany(s => s.Aggregate.GetEvents()));
        }
    }
}