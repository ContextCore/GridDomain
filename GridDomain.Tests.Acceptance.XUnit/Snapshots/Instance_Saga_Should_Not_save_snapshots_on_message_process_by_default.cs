using System;
using System.Threading.Tasks;
using GridDomain.Common;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Tests.Framework;
using GridDomain.Tests.XUnit.Sagas;
using GridDomain.Tests.XUnit.Sagas.SoftwareProgrammingDomain;
using GridDomain.Tests.XUnit.Sagas.SoftwareProgrammingDomain.Events;
using GridDomain.Tools.Repositories.AggregateRepositories;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Acceptance.XUnit.Snapshots
{
    public class Instance_Saga_Should_Not_save_snapshots_on_message_process_by_default : SoftwareProgrammingSagaTest
    {
        public Instance_Saga_Should_Not_save_snapshots_on_message_process_by_default(ITestOutputHelper helper) : base(helper) {}

        [Fact]
        public async Task Given_default_policy()
        {
            var sagaStartEvent = new GotTiredEvent(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

            var res = await
                Node.NewDebugWaiter()
                    .Expect<SagaCreated<SoftwareProgrammingState>>()
                    .Create()
                    .SendToSagas(sagaStartEvent);

            var sagaId = res.Message<SagaCreated<SoftwareProgrammingState>>().SourceId;

            var sagaContinueEvent = new CoffeMakeFailedEvent(Guid.NewGuid(), sagaStartEvent.PersonId, BusinessDateTime.UtcNow, sagaId);

            await Node.NewDebugWaiter()
                    .Expect<SagaReceivedMessage<SoftwareProgrammingState>>()
                    .Create()
                    .SendToSagas(sagaContinueEvent);

            //saving snapshot
            await Task.Delay(200);

            var snapshots =
                await
                    new AggregateSnapshotRepository(AkkaConfig.Persistence.JournalConnectionString,
                                                    Node.AggregateFromSnapshotsFactory).Load<SagaStateAggregate<SoftwareProgrammingState>>(
                                                                                                                                              sagaStartEvent.SagaId);
            //Snapshot_should_be_saved_one_time()
            Assert.Equal(0, snapshots.Length);
        }
    }
}