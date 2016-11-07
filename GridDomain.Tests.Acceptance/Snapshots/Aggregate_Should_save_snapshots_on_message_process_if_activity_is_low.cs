using System;
using System.Linq;
using System.Threading;
using GridDomain.Common;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Tests.CommandsExecution;
using GridDomain.Tests.SampleDomain;
using GridDomain.Tests.SampleDomain.Commands;
using GridDomain.Tests.SampleDomain.Events;
using GridDomain.Tools.Repositories;
using NUnit.Framework;

namespace GridDomain.Tests.Acceptance.Snapshots
{
    [TestFixture]
    class Aggregate_Should_save_snapshots_on_message_process_if_activity_is_low : SampleDomainCommandExecutionTests
    {
        private Guid _aggregateId;
        private AggregateVersion<SampleAggregate>[] _snapshots;
        private int _initialParameter;
        private int _changedParameter;

 

        public Aggregate_Should_save_snapshots_on_message_process_if_activity_is_low():base(false)
        {

        }

        protected override IContainerConfiguration CreateConfiguration()
        {
            return new CustomContainerConfiguration(
                c => base.CreateConfiguration().Register(c),
                c => new AggregateConfiguration<SampleAggregate, SampleAggregatesCommandHandler>(new TestSnapshotsSavePolicy()).Register(c));
        }

        [OneTimeSetUp]
        public void Given_timeout_only_default_policy()
        {
            _aggregateId = Guid.NewGuid();
            _initialParameter = 1;
            var cmd = new CreateSampleAggregateCommand(_initialParameter, _aggregateId);
            GridNode.NewCommandWaiter(Timeout)
                    .Expect<SampleAggregateCreatedEvent>()
                    .Create()
                    .Execute(cmd)
                    .Wait();

            //checking "time-out" rule for policy, snapshots should be saved once on second command
            Thread.Sleep(1000);
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
            Thread.Sleep(TimeSpan.FromSeconds(1));

            _snapshots = new AggregateSnapshotRepository(AkkaConf.Persistence.JournalConnectionString).Load<SampleAggregate>(_aggregateId);
        }

        [Test]
        public void Snapshots_should_be_saved_one_time()
        {
            Assert.AreEqual(1, _snapshots.Length);
        }

        [Test]
        public void Restored_aggregates_should_have_same_ids()
        {
            Assert.True(_snapshots.All(s => s.Aggregate.Id == _aggregateId));
        }

        [Test]
        public void First_snapshot_should_have_parameters_from_second_command()
        {
            Assert.True(_snapshots.First().Aggregate.Value == _changedParameter.ToString());
        }
    }
}