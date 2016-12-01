using System;
using GridDomain.Common;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Tests.CommandsExecution;
using GridDomain.Tests.Framework;
using GridDomain.Tests.SampleDomain;
using GridDomain.Tools.Repositories;
using GridDomain.Tools.Repositories.AggregateRepositories;
using NUnit.Framework;

namespace GridDomain.Tests.Acceptance.Snapshots
{
    [TestFixture]
    class Aggregate_should_recover_from_snapshot : SampleDomainCommandExecutionTests
    {
        private SampleAggregate _aggregate;
        private SampleAggregate _restoredAggregate;
        public Aggregate_should_recover_from_snapshot(): base(false) {}

  //     protected override TimeSpan Timeout => TimeSpan.FromSeconds(1000);

        protected override IContainerConfiguration CreateConfiguration()
        {
            return new CustomContainerConfiguration(
                base.CreateConfiguration(),
                new AggregateConfiguration<SampleAggregate, SampleAggregatesCommandHandler>(
                                                          () => new SnapshotsPersistenceAfterEachMessagePolicy(),
                                                          SampleAggregate.FromSnapshot
                                                          )
                );
        }


        [OneTimeSetUp]
        public void Test()
        {
            _aggregate = new SampleAggregate(Guid.NewGuid(), "test");
            _aggregate.ChangeState(10);
            _aggregate.ClearEvents();

            var repo = new AggregateSnapshotRepository(AkkaConf.Persistence.JournalConnectionString, GridNode.AggregateFromSnapshotsFactory);
            repo.Add(_aggregate);

            _restoredAggregate = LoadAggregate<SampleAggregate>(_aggregate.Id);
        }

        [Test]
        public void Values_should_be_equal()
        {
            Assert.AreEqual(_aggregate.Value, _restoredAggregate.Value);
        }

        [Test]
        public void State_restored_from_snapshot_should_not_have_uncommited_events()
        {
            CollectionAssert.IsEmpty(_restoredAggregate.GetEvents());
        }

        [Test]
        public void Ids_should_be_equal()
        {
            Assert.AreEqual(_aggregate.Id, _restoredAggregate.Id);
        }

    }
}
