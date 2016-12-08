using System;
using System.Linq;
using System.Threading;
using GridDomain.Common;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Node.Actors;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Tests.Framework;
using GridDomain.Tests.Sagas.InstanceSagas;
using GridDomain.Tests.Sagas.SoftwareProgrammingDomain.Events;
using GridDomain.Tests.SampleDomain;
using GridDomain.Tests.SampleDomain.Commands;
using GridDomain.Tests.SampleDomain.Events;
using GridDomain.Tools.Repositories;
using GridDomain.Tools.Repositories.AggregateRepositories;
using NUnit.Framework;

namespace GridDomain.Tests.Acceptance.Snapshots
{
    [TestFixture]
    class Instance_saga_Should_save_snapshots_each_n_messages_according_to_policy : SoftwareProgrammingInstanceSagaTest
    {
        private Guid _sagaId;
        private AggregateVersion<SagaDataAggregate<SoftwareProgrammingSagaData>>[] _snapshots;

        protected override IContainerConfiguration CreateConfiguration()
        {
            return new CustomContainerConfiguration(
                c => base.CreateConfiguration().Register(c),
                c => c.Register(SagaConfiguration.Instance<SoftwareProgrammingSaga,
                                SoftwareProgrammingSagaData,
                                SoftwareProgrammingSagaFactory,
                                GotTiredEvent,
                                SleptWellEvent>(SoftwareProgrammingSaga.Descriptor,
                                () => new SnapshotsPersistencePolicy(TimeSpan.FromSeconds(10),2))));
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
        public void Second_snapshot_should_have_parameters_from_second_command()
        {
            Assert.AreEqual(nameof(SoftwareProgrammingSaga.Sleeping),_snapshots.Skip(1).First().Aggregate.Data.CurrentStateName);
        }

        [Test]
        public void All_snapshots_should_not_have_uncommited_events()
        {
            CollectionAssert.IsEmpty(_snapshots.SelectMany(s => s.Aggregate.GetEvents()));
        }
    }
}