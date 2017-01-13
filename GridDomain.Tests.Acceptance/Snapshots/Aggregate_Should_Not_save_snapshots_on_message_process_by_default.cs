using System;
using System.Threading;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.Node;
using GridDomain.Tests.Unit.CommandsExecution;
using GridDomain.Tests.Unit.SampleDomain;
using GridDomain.Tests.Unit.SampleDomain.Commands;
using GridDomain.Tests.Unit.SampleDomain.Events;
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
        public async Task Given_timeout_only_default_policy()
        {
            _aggregateId = Guid.NewGuid();
            _initialParameter = 1;
            var cmd = new CreateSampleAggregateCommand(_initialParameter, _aggregateId);
            await GridNode.Prepare(cmd)
                          .Expect<SampleAggregateCreatedEvent>()
                          .Execute(Timeout);

            //checking "time-out" rule for policy, snapshots should be saved once on second command
            Thread.Sleep(1000);
            _changedParameter = 2;
            var changeSampleAggregateCommand = new ChangeSampleAggregateCommand(_changedParameter, _aggregateId);

            await GridNode.Prepare(changeSampleAggregateCommand)
                          .Expect<SampleAggregateChangedEvent>()
                          .Execute();

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