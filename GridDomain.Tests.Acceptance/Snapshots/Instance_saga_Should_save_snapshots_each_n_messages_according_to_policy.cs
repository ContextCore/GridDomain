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
    class Instance_saga_Should_save_snapshots_each_n_messages_according_to_policy : SoftwareProgrammingInstanceSagaTest
    {
        private Guid _sagaId;
        private AggregateVersion<SagaStateAggregate<SoftwareProgrammingSagaData>>[] _snapshots;

        protected override IContainerConfiguration CreateConfiguration()
        {
            return new CustomContainerConfiguration(
                c => base.CreateConfiguration().Register(c),
                c => c.Register(SagaConfiguration.Instance<SoftwareProgrammingSaga, SoftwareProgrammingSagaData, SoftwareProgrammingSagaFactory>(
                                SoftwareProgrammingSaga.Descriptor,
                                () => new EachMessageSnapshotsPersistencePolicy())));
        }
        public Instance_saga_Should_save_snapshots_each_n_messages_according_to_policy():base(false)
        {

        }

        [OneTimeSetUp]
        public void Given_default_policy()
        {
            _sagaId = Guid.NewGuid();
            var sagaStartEvent = new GotTiredEvent(_sagaId, Guid.NewGuid(), Guid.NewGuid(),_sagaId);

            var waiter = GridNode.NewWaiter()
                                 .Expect<SagaCreatedEvent<SoftwareProgrammingSagaData>>()
                                 .Create();

            Publisher.Publish(sagaStartEvent);
            waiter.Wait();

            var sagaContinueEvent = new CoffeMakeFailedEvent(_sagaId, 
                                                             sagaStartEvent.PersonId,
                                                             BusinessDateTime.UtcNow,
                                                            _sagaId);

            var waiterB = GridNode.NewWaiter(DefaultTimeout)
                                  .Expect<SagaMessageReceivedEvent<SoftwareProgrammingSagaData>>()
                                  .Create();

            Publisher.Publish(sagaContinueEvent);

            waiterB.Wait();

   
            //saving snapshot
            Thread.Sleep(200);

            _snapshots = new AggregateSnapshotRepository(AkkaConf.Persistence.JournalConnectionString, GridNode.AggregateFromSnapshotsFactory)
                                .Load<SagaStateAggregate<SoftwareProgrammingSagaData>>(sagaStartEvent.SagaId);
        }

        //saving on each message, maximum on each command
        [Test]
        public void Snapshots_should_be_saved_two_times()
        {
            Assert.AreEqual(2, _snapshots.Length);
        }

        [Test]
        public void Restored_saga_state_should_have_correct_ids()
        {
            Assert.True(_snapshots.All(s => s.Aggregate.Id == _sagaId));
        }

        [Test]
        public void First_snapshot_should_have_state_from_first_event()
        {
            Assert.AreEqual(nameof(SoftwareProgrammingSaga.MakingCoffee),_snapshots.First().Aggregate.Data.CurrentStateName);
        }

        [Test]
        public void Last_snapshot_should_have_parameters_from_last_command()
        {
            Assert.AreEqual(nameof(SoftwareProgrammingSaga.Sleeping),_snapshots.Last().Aggregate.Data.CurrentStateName);
        }

        [Test]
        public void All_snapshots_should_not_have_uncommited_events()
        {
            CollectionAssert.IsEmpty(_snapshots.SelectMany(s => s.Aggregate.GetEvents()));
        }
    }
}