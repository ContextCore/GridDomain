using System;
using System.Linq;
using System.Threading.Tasks;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Tests.Acceptance.XUnit.EventsUpgrade;
using GridDomain.Tests.Framework;
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
            var sagaId = Guid.NewGuid();
            var sagaStartEvent = new GotTiredEvent(sagaId, Guid.NewGuid(), Guid.NewGuid(), sagaId);

            await
                Node.NewDebugWaiter()
                    .Expect<SagaCreatedEvent<SoftwareProgrammingSagaState>>()
                    .Create()
                    .SendToSagas(sagaStartEvent);

            //var saga = await Node.LookupSagaActor<SoftwareProgrammingSaga, SoftwareProgrammingSagaData>(sagaId);
            //saga.Tell(NotifyOnPersistenceEvents.Instance);

            var sagaContinueEvent = new CoffeMakeFailedEvent(sagaId, sagaStartEvent.PersonId, BusinessDateTime.UtcNow, sagaId);

            await
                Node.NewDebugWaiter()
                    .Expect<SagaMessageReceivedEvent<SoftwareProgrammingSagaState>>()
                    .Create()
                    .SendToSagas(sagaContinueEvent);

            var sagaContinueEventB =
                new Fault<GoSleepCommand>(new GoSleepCommand(sagaStartEvent.PersonId, sagaStartEvent.LovelySofaId),
                                          new Exception(),
                                          typeof(object),
                                          sagaId,
                                          BusinessDateTime.Now);

            await
                Node.NewDebugWaiter()
                    .Expect<SagaMessageReceivedEvent<SoftwareProgrammingSagaState>>()
                    .Create()
                    .SendToSagas(sagaContinueEventB);

            //   FishForMessage<Persisted>(m => m.Event is SaveSnapshotSuccess);

            await Task.Delay(500);

            var snapshots =
                await
                    new AggregateSnapshotRepository(AkkaConfig.Persistence.JournalConnectionString,
                                                    Node.AggregateFromSnapshotsFactory).Load<SagaStateAggregate<SoftwareProgrammingSagaState>>(
                                                                                                                                              sagaStartEvent.SagaId);

            //saving on each message, maximum on each command
            //Snapshots_should_be_saved_two_times
            //4 events in total, two saves of snapshots due to policy saves on each two events
            Assert.Equal(2, snapshots.Length);
            //First_snapshot_should_have_state_from_first_event
            Assert.Equal(nameof(SoftwareProgrammingSaga.MakingCoffee), snapshots.First().Aggregate.SagaState.CurrentStateName);
            //Last_snapshot_should_have_parameters_from_last_command()
            Assert.Equal(nameof(SoftwareProgrammingSaga.Coding), snapshots.Last().Aggregate.SagaState.CurrentStateName);

            //Restored_saga_state_should_have_correct_ids
            Assert.True(snapshots.All(s => s.Aggregate.Id == sagaId));
            //All_snapshots_should_not_have_uncommited_events()
            Assert.Empty(snapshots.SelectMany(s => s.Aggregate.GetEvents()));
        }
    }
}