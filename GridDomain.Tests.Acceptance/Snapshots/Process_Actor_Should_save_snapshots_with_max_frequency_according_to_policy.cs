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
using Serilog;
using Xunit;
using Xunit.Abstractions;
using GridDomain.Transport.Remote;

namespace GridDomain.Tests.Acceptance.Snapshots
{
    public class Process_Actor_Should_save_snapshots_with_max_frequency_according_to_policy : NodeTestKit
    {
        private static readonly Guid ProcessFixedId = Guid.NewGuid();

        public Process_Actor_Should_save_snapshots_with_max_frequency_according_to_policy(ITestOutputHelper output)
            : base(output,
                new KnownProcessIdFixture(ProcessFixedId).UseSqlPersistence()
                                                         .InitSnapshots(2, TimeSpan.FromSeconds(10))
                                                         .IgnoreCommands()) { }

        private class KnownProcessIdFixture : SoftwareProgrammingProcessManagerFixture
        {
            class FixedProcessIdFactory : SoftwareProgrammingProcessManagerFactory
            {
                private readonly Guid _staticId;

                public FixedProcessIdFactory(ILogger log, Guid staticId) : base(log)
                {
                    _staticId = staticId;
                }

                public override IProcessManager<SoftwareProgrammingState> CreateNew(GotTiredEvent message, Guid? processId = null)
                {
                    return base.CreateNew(message, _staticId);
                }
            }

            class FixedIdProcessDependencyFactory : DefaultProcessManagerDependencyFactory<SoftwareProgrammingState>
            {
                public FixedIdProcessDependencyFactory(ILogger log, Guid staticId) : base(new FixedProcessIdFactory(log, staticId), SoftwareProgrammingProcess.Descriptor) { }
            }

            public KnownProcessIdFixture(Guid fixedId)
            {
                ProcessConfiguration.SoftwareProgrammingProcessManagerDependenciesFactory =
                    new FixedIdProcessDependencyFactory(Logger, fixedId);
            }
        }

        [Fact]
        public async Task Given_default_policy()
        {
            var startEvent = new GotTiredEvent(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

            var fixedProcess = await Node.WarmUpProcessManager<SoftwareProgrammingProcess>(ProcessFixedId);
            await fixedProcess.Info.Ref.Ask<SubscribeAck>(new NotifyOnPersistenceEvents(TestActor));

            var resTask = Node.NewDebugWaiter()
                              .Expect<ProcessReceivedMessage<SoftwareProgrammingState>>()
                              .Create()
                              .SendToProcessManagers(startEvent);

            FishForMessage<SaveSnapshotSuccess>(t => true);

            var processId = (await resTask).Message<ProcessReceivedMessage<SoftwareProgrammingState>>()
                                           .SourceId;

            var continueEvent = new CoffeMakeFailedEvent(processId, startEvent.PersonId, BusinessDateTime.UtcNow, processId);

            //to avoid racy state receiving expected message from processing GotTiredEvent 
            await Node.NewDebugWaiter()
                      .Expect<ProcessReceivedMessage<SoftwareProgrammingState>>(e 
                      => (e.Message as IMessageMetadataEnvelop)?.Message is CoffeMakeFailedEvent)
                      .Create()
                      .SendToProcessManagers(continueEvent, processId);

          
            await Node.KillProcessManager<SoftwareProgrammingProcess, SoftwareProgrammingState>(ProcessFixedId);

            this.Log.Info("Enforced additional snapshot save & delete");

            var snapshots = await new AggregateSnapshotRepository(AkkaConfig.Persistence.JournalConnectionString,
                new AggregateFactory()).Load<ProcessStateAggregate<SoftwareProgrammingState>>(processId);

            //Snapshot_should_be_saved_one_time
            Assert.Equal(1, snapshots.Length);
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