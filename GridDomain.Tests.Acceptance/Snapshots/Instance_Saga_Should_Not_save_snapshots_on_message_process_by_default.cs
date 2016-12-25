using System;
using System.Linq;
using System.Threading;
using GridDomain.Common;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Tests.Framework;
using GridDomain.Tests.Unit.Sagas.InstanceSagas;
using GridDomain.Tests.Unit.Sagas.SoftwareProgrammingDomain.Events;
using GridDomain.Tools.Repositories;
using GridDomain.Tools.Repositories.AggregateRepositories;
using NUnit.Framework;

namespace GridDomain.Tests.Acceptance.Snapshots
{
    [TestFixture]
    class Instance_Saga_Should_Not_save_snapshots_on_message_process_by_default: SoftwareProgrammingInstanceSagaTest
    {
        private Guid _sagaId;
        private AggregateVersion<SagaDataAggregate<SoftwareProgrammingSagaData>>[] _snapshots;

        public Instance_Saga_Should_Not_save_snapshots_on_message_process_by_default():base(false)
        {

        }

        [OneTimeSetUp]
        public void Given_default_policy()
        {
            _sagaId = Guid.NewGuid();
            var sagaStartEvent = new GotTiredEvent(_sagaId, Guid.NewGuid(), Guid.NewGuid(), _sagaId);

            var waiter = GridNode.NewWaiter(TimeSpan.FromSeconds(100))
                                 .Expect<SagaCreatedEvent<SoftwareProgrammingSagaData>>()
                                 .Create();

            Publisher.Publish(sagaStartEvent);
            waiter.Wait();

            Thread.Sleep(1000);

            var sagaContinueEvent = new CoffeMakeFailedEvent(_sagaId,
                                                             sagaStartEvent.PersonId,
                                                             BusinessDateTime.UtcNow,
                                                            _sagaId);

            var waiterB = GridNode.NewWaiter(Timeout)
                                  .Expect<SagaTransitionEvent<SoftwareProgrammingSagaData>>()
                                  .Create();

            Publisher.Publish(sagaContinueEvent);

            waiterB.Wait();


            //saving snapshot
            Thread.Sleep(200);

            _snapshots = new AggregateSnapshotRepository(AkkaConf.Persistence.JournalConnectionString, GridNode.AggregateFromSnapshotsFactory)
                                .Load<SagaDataAggregate<SoftwareProgrammingSagaData>>(sagaStartEvent.SagaId);
        }

        [Test]
        public void Snapshot_should_be_saved_one_time()
        {
            Assert.AreEqual(0, _snapshots.Length);
        }
    }
}