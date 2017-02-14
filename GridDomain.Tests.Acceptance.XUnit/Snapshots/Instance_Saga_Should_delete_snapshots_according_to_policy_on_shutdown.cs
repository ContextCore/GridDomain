using System;
using System.Linq;
using System.Security.Policy;
using System.Threading;
using System.Threading.Tasks;
using Akka.Actor;
using GridDomain.Common;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Logging;
using GridDomain.Node.Actors;
using GridDomain.Node.Configuration.Composition;
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
    public class Instance_Saga_Should_delete_snapshots_according_to_policy_on_shutdown : NodeTestKit
    {
        class SagaSnapshotsFixture : SoftwareProgrammingSagaFixture
        {
            private readonly SnapshotsPersistencePolicy _snapshotsPersistencePolicy =
                new SnapshotsPersistencePolicy(TimeSpan.FromMinutes(10), 1, 2);

            public SagaSnapshotsFixture()
            {
                Add(
                    new CustomContainerConfiguration(
                        c =>
                            c.Register(
                                SagaConfiguration
                                    .Instance
                                    <SoftwareProgrammingSaga, SoftwareProgrammingSagaData, SoftwareProgrammingSagaFactory>(
                                        SoftwareProgrammingSaga.Descriptor,
                                        () => _snapshotsPersistencePolicy))));
                Console.WriteLine(_snapshotsPersistencePolicy.ToPropsString());
            }
        }

        [Fact]
        public async Task Given_save_on_each_message_policy_and_keep_2_snapshots()
        {
            var sagaId = Guid.NewGuid();
            var sagaStartEvent = new GotTiredEvent(sagaId, Guid.NewGuid(), Guid.NewGuid(), sagaId);

            await Node.NewDebugWaiter()
                      .Expect<SagaCreatedEvent<SoftwareProgrammingSagaData>>()
                      .Create()
                      .SendToSagas(sagaStartEvent);


            var sagaActorRef =
                await Node.System.LookupSagaActor<SoftwareProgrammingSaga, SoftwareProgrammingSagaData>(sagaId);

            sagaActorRef.Tell(new NotifyOnPersistenceEvents(TestActor), TestActor);

            var sagaContinueEventA = new CoffeMakeFailedEvent(sagaId,
                sagaStartEvent.PersonId,
                BusinessDateTime.UtcNow,
                sagaId);

            var sagaContinueEventB = new SleptWellEvent(sagaId, sagaStartEvent.LovelySofaId, sagaId);

            await Node.NewDebugWaiter()
                      .Expect<SagaMessageReceivedEvent<SoftwareProgrammingSagaData>>(
                          e => (e.Message as CoffeMakeFailedEvent)?.SourceId == sagaId)
                      .Create()
                      .SendToSagas(sagaContinueEventA);

            await Node.NewDebugWaiter()
                      .Expect<SagaMessageReceivedEvent<SoftwareProgrammingSagaData>>(
                          e => (e.Message as SleptWellEvent)?.SourceId == sagaId)
                      .Create()
                      .SendToSagas(sagaContinueEventB);


            Watch(sagaActorRef);
            sagaActorRef.Tell(GracefullShutdownRequest.Instance, TestActor);

            FishForMessage<Terminated>(m => true);
            await Task.Delay(1000);
            var snapshots =
                await
                    new AggregateSnapshotRepository(Fixture.AkkaConfig.Persistence.JournalConnectionString,
                        Node.AggregateFromSnapshotsFactory).Load<SagaStateAggregate<SoftwareProgrammingSagaData>>(sagaId);

            //Only_two_Snapshots_should_left()
            Assert.Equal(2, snapshots.Length);
            // Restored_aggregates_should_have_same_ids()
            Assert.True(snapshots.All(s => s.Aggregate.Id == sagaId));
            //Last_Snapshots_should_have_coding_state_from_last_event()
            Assert.Equal(nameof(SoftwareProgrammingSaga.Coding),
                snapshots.Last()
                         .Aggregate.Data.CurrentStateName);
            // First_Snapshots_should_have_coding_state_from_first_event()
            Assert.Equal(nameof(SoftwareProgrammingSaga.Sleeping),
                snapshots.First()
                         .Aggregate.Data.CurrentStateName);
            //All_snapshots_should_not_have_uncommited_events()
            Assert.Empty(snapshots.SelectMany(s => s.Aggregate.GetEvents()));
        }

        public Instance_Saga_Should_delete_snapshots_according_to_policy_on_shutdown(ITestOutputHelper output)
            : base(output, new SagaSnapshotsFixture()) {}
    }
}