using System;
using System.Linq;
using System.Threading;
using GridDomain.Common;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Node.Actors;
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
    class Instance_Saga_Should_save_snapshots_on_message_process_if_activity_is_low: SoftwareProgrammingInstanceSagaTest
    {
        private Guid _sagaId;
        private AggregateVersion<SagaDataAggregate<SoftwareProgrammingSagaData>>[] _snapshots;

        public Instance_Saga_Should_save_snapshots_on_message_process_if_activity_is_low():base(false)
        {

        }

        protected override IContainerConfiguration CreateConfiguration()
        {
            return new CustomContainerConfiguration(
                base.CreateConfiguration(),
                SagaConfiguration.Instance<SoftwareProgrammingSaga,
                                           SoftwareProgrammingSagaData>(
                                           new SoftwareProgrammingSagaFactory(),
                                           SoftwareProgrammingSaga.Descriptor, 
                                           () => new SnapshotsPersistencePolicy(TimeSpan.FromSeconds(1),10,1)
                                           ));
        }

        [OneTimeSetUp]
        public void Given_default_policy()
        {
            _sagaId = Guid.NewGuid();
            var sagaStartEvent = new GotTiredEvent(_sagaId, Guid.NewGuid(), Guid.NewGuid(), _sagaId);

            var waiter = GridNode.NewWaiter()
                                 .Expect<SagaCreatedEvent<SoftwareProgrammingSagaData>>()
                                 .Create();

            Publisher.Publish(sagaStartEvent);
            waiter.Wait();

            Thread.Sleep(1000);

            var sagaContinueEvent = new CoffeMakeFailedEvent(_sagaId,
                                                             sagaStartEvent.PersonId,
                                                             BusinessDateTime.UtcNow,
                                                            _sagaId);

            var waiterB = GridNode.NewWaiter()
                                  .Expect<SagaMessageReceivedEvent<SoftwareProgrammingSagaData>>()
                                  .Create();

            Publisher.Publish(sagaContinueEvent);

            waiterB.Wait();


            //saving snapshot
            Thread.Sleep(200);

            _snapshots = new AggregateSnapshotRepository(AkkaConf.Persistence.JournalConnectionString, GridNode.AggregateFromSnapshotsFactory)
                                .Load<SagaDataAggregate<SoftwareProgrammingSagaData>>(sagaStartEvent.SagaId);
        }

        [Test]
        [Ignore("for a while")]
        public void Snapshot_should_be_saved_one_time()
        {
            Assert.AreEqual(1, _snapshots.Length);
        }

        [Test]
        public void Restored_saga_state_should_have_correct_ids()
        {
            Assert.True(_snapshots.All(s => s.Aggregate.Id == _sagaId));
        }


        [Test]
        public void Snapshot_should_have_parameters_from_second_command()
        {
            Assert.AreEqual(nameof(SoftwareProgrammingSaga.Sleeping), _snapshots.First().Aggregate.Data.CurrentStateName);
        }

        [Test]
        public void All_snapshots_should_not_have_uncommited_events()
        {
            CollectionAssert.IsEmpty(_snapshots.SelectMany(s => s.Aggregate.GetEvents()));
        }
    }
}