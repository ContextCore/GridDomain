using System;
using System.Threading;
using GridDomain.Node;
using GridDomain.Tests.CommandsExecution;
using GridDomain.Tests.SampleDomain;
using GridDomain.Tests.SampleDomain.Commands;
using GridDomain.Tests.SampleDomain.Events;
using GridDomain.Tools.Repositories;
using GridDomain.Tools.Repositories.AggregateRepositories;
using NUnit.Framework;

namespace GridDomain.Tests.Acceptance.Snapshots
{
    [TestFixture]
    class Aggregate_Should_Not_save_snapshots_on_message_process_by_default : SampleDomainCommandExecutionTests
    {
        private Guid _aggregateId;
        private AggregateVersion<SampleAggregate>[] _snapshots;
        private int _initialParameter;
        private int _changedParameter;



        public Aggregate_Should_Not_save_snapshots_on_message_process_by_default() : base(false)
        {

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

            _snapshots = new AggregateSnapshotRepository(AkkaConf.Persistence.JournalConnectionString, GridNode.AggregateFromSnapshotsFactory).Load<SampleAggregate>(_aggregateId);
        }

        [Test]
        public void Snapshots_should_be_saved_one_time()
        {
            Assert.AreEqual(0, _snapshots.Length);
        }
    }
}