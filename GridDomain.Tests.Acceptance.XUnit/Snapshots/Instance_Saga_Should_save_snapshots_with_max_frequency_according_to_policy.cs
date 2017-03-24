using System;
using System.Linq;
using System.Threading.Tasks;
using GridDomain.Common;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Tests.Acceptance.XUnit.EventsUpgrade;
using GridDomain.Tests.Framework;
using GridDomain.Tests.XUnit;
using GridDomain.Tests.XUnit.Sagas;
using GridDomain.Tests.XUnit.Sagas.SoftwareProgrammingDomain;
using GridDomain.Tests.XUnit.Sagas.SoftwareProgrammingDomain.Events;
using GridDomain.Tools.Repositories.AggregateRepositories;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Acceptance.XUnit.Snapshots
{
    public class Instance_Saga_Should_save_snapshots_with_max_frequency_according_to_policy : NodeTestKit
    {
        public Instance_Saga_Should_save_snapshots_with_max_frequency_according_to_policy(ITestOutputHelper output)
            : base(
                   output,
                   new SoftwareProgrammingSagaFixture {InMemory = false}.InitSoftwareProgrammingSagaSnapshots(2,
                                                                                                              TimeSpan.FromSeconds(10)).IgnoreCommands()) {}

        [Fact]
        public async Task Given_default_policy()
        {
            var sagaId = Guid.NewGuid();
            var sagaStartEvent = new GotTiredEvent(sagaId, Guid.NewGuid(), Guid.NewGuid());

            await
                Node.NewDebugWaiter()
                    .Expect<SagaCreatedEvent<SoftwareProgrammingSagaState>>()
                    .Create()
                    .SendToSagas(sagaStartEvent, sagaId);

            //wait some time, allowing first snapshots to be saved
            await Task.Delay(200);
            var sagaContinueEvent = new CoffeMakeFailedEvent(sagaId, sagaStartEvent.PersonId, BusinessDateTime.UtcNow);

            //send text event
            await
                Node.NewDebugWaiter()
                    .Expect<SagaMessageReceivedEvent<SoftwareProgrammingSagaState>>()
                    .Create()
                    .SendToSagas(sagaContinueEvent, sagaId);

            //wait some time, second snapshots should not be saved due to max frequency in snapshotting policy
            await Task.Delay(200);

            var snapshots =
                await
                    new AggregateSnapshotRepository(AkkaConfig.Persistence.JournalConnectionString,
                                                    Node.AggregateFromSnapshotsFactory).Load<SagaStateAggregate<SoftwareProgrammingSagaState>>(sagaId);

            //Snapshot_should_be_saved_one_time
            Assert.Equal(1, snapshots.Length);
            //Restored_saga_state_should_have_correct_ids
            Assert.True(snapshots.All(s => s.Aggregate.Id == sagaId));
            //Snapshot_should_have_parameters_from_first_event
            Assert.Equal(nameof(SoftwareProgrammingSaga.MakingCoffee), snapshots.First().Aggregate.SagaState.CurrentStateName);
            //All_snapshots_should_not_have_uncommited_events
            Assert.Empty(snapshots.SelectMany(s => s.Aggregate.GetEvents()));
        }
    }
}