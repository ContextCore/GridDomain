using System;
using System.Linq;
using System.Security.Policy;
using System.Threading;
using System.Threading.Tasks;
using GridDomain.Common;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Node.Actors;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Tests.Framework;
using GridDomain.Tests.XUnit;
using GridDomain.Tests.XUnit.Sagas.SoftwareProgrammingDomain;
using GridDomain.Tests.XUnit.Sagas.SoftwareProgrammingDomain.Events;
using GridDomain.Tools.Repositories.AggregateRepositories;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Acceptance.XUnit.Snapshots
{
    public class Instance_Saga_Should_save_snapshots_on_message_process_if_activity_is_low : NodeTestKit
    {

        class SagaFixture : SampleDomainFixture
        {
            public SagaFixture()
            {
                Add(
                    new CustomContainerConfiguration(
                        SagaConfiguration
                            .Instance<SoftwareProgrammingSaga, SoftwareProgrammingSagaData, SoftwareProgrammingSagaFactory>(
                                SoftwareProgrammingSaga.Descriptor,
                                () => new SnapshotsPersistencePolicy(TimeSpan.FromSeconds(1), 10, 1))));
            }
        }

        [Fact]
        public async Task Given_default_policy()
        {
            var sagaId = Guid.NewGuid();
            var sagaStartEvent = new GotTiredEvent(sagaId, Guid.NewGuid(), Guid.NewGuid(), sagaId);

            await Node.NewDebugWaiter()
                      .Expect<SagaCreatedEvent<SoftwareProgrammingSagaData>>()
                      .Create()
                      .SendToSagas(sagaStartEvent);

            await Task.Delay(1000);

            var sagaContinueEvent = new CoffeMakeFailedEvent(sagaId,
                sagaStartEvent.PersonId,
                BusinessDateTime.UtcNow,
                sagaId);

            await Node.NewDebugWaiter()
                      .Expect<SagaMessageReceivedEvent<SoftwareProgrammingSagaData>>()
                      .Create()
                      .SendToSagas(sagaContinueEvent);

            //saving snapshot
            await Task.Delay(200);

            var snapshots =
                await
                    new AggregateSnapshotRepository(Fixture.AkkaConfig.Persistence.JournalConnectionString,
                        Node.AggregateFromSnapshotsFactory).Load<SagaStateAggregate<SoftwareProgrammingSagaData>>(
                            sagaStartEvent.SagaId);
            //Snapshot_should_be_saved_one_time()
            Assert.Equal(1, snapshots.Length);
            //Restored_saga_state_should_have_correct_ids()
            Assert.True(snapshots.All(s => s.Aggregate.Id == sagaId));
            //Snapshot_should_have_parameters_from_second_command()
            Assert.Equal(nameof(SoftwareProgrammingSaga.Sleeping),
                snapshots.First()
                          .Aggregate.Data.CurrentStateName);
            //All_snapshots_should_not_have_uncommited_events()
            Assert.Empty(snapshots.SelectMany(s => s.Aggregate.GetEvents()));
        }

        public Instance_Saga_Should_save_snapshots_on_message_process_if_activity_is_low(ITestOutputHelper output)
            : base(output, new SagaFixture()) {}
    }
}