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
    public class Instance_saga_Should_save_snapshots_each_n_messages_according_to_policy : NodeTestKit
    {
        class SagaFixture : SampleDomainFixture
        {
            public SagaFixture()
            {
                InMemory = false;
                Add(
                    new CustomContainerConfiguration(
                        c =>
                            c.Register(
                                SagaConfiguration
                                    .Instance
                                    <SoftwareProgrammingSaga, SoftwareProgrammingSagaData, SoftwareProgrammingSagaFactory>(
                                        SoftwareProgrammingSaga.Descriptor,
                                        () => new EachMessageSnapshotsPersistencePolicy()))));
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

            var sagaContinueEvent = new CoffeMakeFailedEvent(sagaId, sagaStartEvent.PersonId, BusinessDateTime.UtcNow, sagaId);

            await Node.NewDebugWaiter()
                      .Expect<SagaMessageReceivedEvent<SoftwareProgrammingSagaData>>()
                      .Create()
                      .SendToSagas(sagaContinueEvent);

            //saving snapshot
            await Task.Delay(200);

            var snapshots =
                await
                    new AggregateSnapshotRepository(AkkaConfig.Persistence.JournalConnectionString,
                        Node.AggregateFromSnapshotsFactory).Load<SagaStateAggregate<SoftwareProgrammingSagaData>>(
                            sagaStartEvent.SagaId);

            //saving on each message, maximum on each command
            //Snapshots_should_be_saved_two_times()
            Assert.Equal(2, snapshots.Length);
            //Restored_saga_state_should_have_correct_ids()
            Assert.True(snapshots.All(s => s.Aggregate.Id == sagaId));
            //First_snapshot_should_have_state_from_first_event()
            Assert.Equal(nameof(SoftwareProgrammingSaga.MakingCoffee),
                snapshots.First()
                         .Aggregate.Data.CurrentStateName);
            //Last_snapshot_should_have_parameters_from_last_command()
            Assert.Equal(nameof(SoftwareProgrammingSaga.Sleeping),
                snapshots.Last()
                         .Aggregate.Data.CurrentStateName);
            //All_snapshots_should_not_have_uncommited_events()
            Assert.Empty(snapshots.SelectMany(s => s.Aggregate.GetEvents()));
        }

        public Instance_saga_Should_save_snapshots_each_n_messages_according_to_policy(ITestOutputHelper output)
            : base(output, new SagaFixture()) {}
    }
}