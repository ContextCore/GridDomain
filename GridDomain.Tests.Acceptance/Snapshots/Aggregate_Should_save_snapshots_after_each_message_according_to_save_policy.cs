using System;
using System.Linq;
using System.Threading;
using GridDomain.Common;
using GridDomain.Node.Actors;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Tests.Framework;
using GridDomain.Tests.Unit.CommandsExecution;
using GridDomain.Tests.Unit.SampleDomain;
using GridDomain.Tests.Unit.SampleDomain.Commands;
using GridDomain.Tests.Unit.SampleDomain.Events;
using GridDomain.Tools.Repositories;
using GridDomain.Tools.Repositories.AggregateRepositories;
using NUnit.Framework;
using Ploeh.AutoFixture;

namespace GridDomain.Tests.Acceptance.Snapshots
{
    [TestFixture]
    class Aggregate_Should_save_snapshots_after_each_message_according_to_save_policy: SampleDomainCommandExecutionTests
    {
        private Guid _aggregateId;
        private AggregateVersion<SampleAggregate>[] _snapshots;
        private int _initialParameter;
        private int _changedParameter;

        public Aggregate_Should_save_snapshots_after_each_message_according_to_save_policy():base(false)
        {
            
        }

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

        protected override TimeSpan Timeout { get; } = TimeSpan.FromMinutes(10);

        [OneTimeSetUp]
        public void Given_default_policy()
        {
            _aggregateId = Guid.NewGuid();
            _initialParameter = 1;
            var cmd = new CreateSampleAggregateCommand(_initialParameter,_aggregateId);
            GridNode.NewCommandWaiter(Timeout)
                    .Expect<SampleAggregateCreatedEvent>()
                    .Create()
                    .Execute(cmd)
                    .Wait();

            _changedParameter = 2;
            var changeCmds = new[]
            {
                new ChangeSampleAggregateCommand(_changedParameter, _aggregateId)
            };

            GridNode.NewCommandWaiter(Timeout)
                    .Expect<SampleAggregateChangedEvent>()
                    .Create()
                    .Execute(changeCmds)
                    .Wait();

            Thread.Sleep(100); 

            _snapshots = new AggregateSnapshotRepository(AkkaConf.Persistence.JournalConnectionString, GridNode.AggregateFromSnapshotsFactory).Load<SampleAggregate>(_aggregateId);
        }

        [Test]
        public void Snapshots_should_be_saved_two_times()
        {
            Assert.AreEqual(2, _snapshots.Length);
        }

        [Test]
        public void Restored_aggregates_should_have_same_ids()
        {
           Assert.True(_snapshots.All(s => s.Aggregate.Id == _aggregateId));
        }

        [Test]
        public void First_snapshot_should_have_parameters_from_first_command()
        {
            Assert.AreEqual(_initialParameter.ToString(), _snapshots.First().Aggregate.Value);
        }

        [Test]
        public void Second_snapshot_should_have_parameters_from_second_command()
        {
            Assert.AreEqual(_changedParameter.ToString(), _snapshots.Skip(1).First().Aggregate.Value);
        }

        [Test]
        public void All_snapshots_should_not_have_uncommited_events()
        {
            CollectionAssert.IsEmpty(_snapshots.SelectMany(s => s.Aggregate.GetEvents()));
        }
    }
}