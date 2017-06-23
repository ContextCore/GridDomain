using System;
using System.Linq;
using System.Threading.Tasks;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Tests.Acceptance.XUnit.EventsUpgrade;
using GridDomain.Tests.Common;
using GridDomain.Tests.XUnit;
using GridDomain.Tests.XUnit.Sagas;
using GridDomain.Tests.XUnit.Sagas.SoftwareProgrammingDomain;
using GridDomain.Tests.XUnit.Sagas.SoftwareProgrammingDomain.Commands;
using GridDomain.Tests.XUnit.Sagas.SoftwareProgrammingDomain.Events;
using GridDomain.Tools.Repositories.AggregateRepositories;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Acceptance.XUnit.Snapshots
{
    public class Instance_saga_Should_save_snapshots_each_n_messages_according_to_policy : NodeTestKit
    {
        public Instance_saga_Should_save_snapshots_each_n_messages_according_to_policy(ITestOutputHelper output)
            : base(
                   output,
                   new SoftwareProgrammingSagaFixture {InMemory = false}.IgnoreCommands()
                                                                        .InitSoftwareProgrammingSagaSnapshots(5,
                                                                                                              TimeSpan.FromMilliseconds(5),
                                                                                                              2)) {}

        [Fact]
        public async Task Given_default_policy()
        {
            var sagaStartEvent = new GotTiredEvent(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

            var res = await Node.NewDebugWaiter()
                                .Expect<SagaCreated<SoftwareProgrammingState>>()
                                .Create()
                                .SendToSagas(sagaStartEvent);

            var sagaId = res.Message<SagaCreated<SoftwareProgrammingState>>().SourceId;

            var sagaContinueEvent = new CoffeMakeFailedEvent(sagaId, sagaStartEvent.PersonId, BusinessDateTime.UtcNow, sagaId);

            await Node.NewDebugWaiter()
                      .Expect<SagaReceivedMessage<SoftwareProgrammingState>>()
                      .Create()
                      .SendToSagas(sagaContinueEvent);

            var sagaContinueEventB =
                new Fault<GoSleepCommand>(new GoSleepCommand(sagaStartEvent.PersonId, sagaStartEvent.LovelySofaId),
                                          new Exception(),
                                          typeof(object),
                                          sagaId,
                                          BusinessDateTime.Now);

            await Node.NewDebugWaiter()
                      .Expect<SagaReceivedMessage<SoftwareProgrammingState>>()
                      .Create()
                      .SendToSagas(sagaContinueEventB);

            await Task.Delay(500);

            var snapshots = await new AggregateSnapshotRepository(AkkaConfig.Persistence.JournalConnectionString,
                                                                  Node.AggregateFromSnapshotsFactory)
                                .Load<SagaStateAggregate<SoftwareProgrammingState>>(sagaId);

            //saving on each message, maximum on each command
            //Snapshots_should_be_saved_two_times
            //4 events in total, two saves of snapshots due to policy saves on each two events
            Assert.Equal(2, snapshots.Length);
            //First_snapshot_should_have_state_from_first_event
            Assert.Equal(nameof(SoftwareProgrammingProcess.MakingCoffee), snapshots.First().Aggregate.State.CurrentStateName);
            //Last_snapshot_should_have_parameters_from_last_command()
            Assert.Equal(nameof(SoftwareProgrammingProcess.Coding), snapshots.Last().Aggregate.State.CurrentStateName);

            //Restored_saga_state_should_have_correct_ids
            Assert.True(snapshots.All(s => s.Aggregate.Id == sagaId));
            //All_snapshots_should_not_have_uncommited_events()
            Assert.Empty(snapshots.SelectMany(s => s.Aggregate.GetEvents()));
        }
    }
}