using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Configuration;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.Node;
using GridDomain.Node.Actors;
using GridDomain.Node.Configuration.Akka;
using GridDomain.Node.Configuration.Akka.Hocon;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Tests.Framework;
using GridDomain.Tests.Framework.Configuration;
using GridDomain.Tests.Unit.CommandsExecution;
using GridDomain.Tests.Unit.SampleDomain;
using GridDomain.Tests.Unit.SampleDomain.Commands;
using GridDomain.Tests.Unit.SampleDomain.Events;
using GridDomain.Tools.Repositories.AggregateRepositories;
using NUnit.Framework;

namespace GridDomain.Tests.Acceptance.Snapshots
{
    [TestFixture]
    [Ignore("Not ready yet")]
    class Aggregate_should_save_snapshot_after_all_events_persisted : SampleDomainCommandExecutionTests
    {

        class SlowDomainJournal : SqlDomainJournal
        {
            public SlowDomainJournal(Config config) : base(config)
            {
            }

            protected override async Task<IImmutableList<Exception>> WriteMessagesAsync(IEnumerable<Akka.Persistence.AtomicWrite> messages)
            {
             //   await Task.Delay(TimeSpan.FromSeconds(5));
                //return await base.WriteMessagesAsync(messages);
                return await Task.FromResult(ImmutableList<Exception>.Empty);
            }
        }


        class TestAkkaConfiguration : AutoTestAkkaConfiguration
        {
            public override string ToStandAloneSystemConfig()
            {
             var cfg = new RootConfig(
                    new LogConfig(LogVerbosity.Info, false),
                    new StandAloneConfig(Network),
                    new PersistenceConfig(new PersistenceJournalConfig(Persistence, new DomainEventAdaptersConfig(), typeof(SlowDomainJournal)),
                                          new PersistenceSnapshotConfig(this)));
                      return cfg.Build();
            }
     
        }

        private Guid _aggregateId;
        private AggregateVersion<SampleAggregate>[] _snapshots;
        private readonly int[] _parameters = new int[5];

        public Aggregate_should_save_snapshot_after_all_events_persisted(): base(false, new TestAkkaConfiguration())
        {

        }

        protected override IContainerConfiguration CreateConfiguration()
        {
            return new CustomContainerConfiguration(
                base.CreateConfiguration(),
                new AggregateConfiguration<SampleAggregate, SampleAggregatesCommandHandler>(
                    () => new SnapshotsPersistenceAfterEachMessagePolicy(2),
                    SampleAggregate.FromSnapshot)
                );
        }


        [OneTimeSetUp]
        public async Task Given_save_on_each_message_policy_and_keep_2_snapshots()
        {
            _aggregateId = Guid.NewGuid();
            var cmd = new CreateSampleAggregateCommand(1, _aggregateId);

            await GridNode.Prepare(cmd)
                          .Expect<SampleAggregateCreatedEvent>()
                          .Execute();

            var aggregateActorRef = LookupAggregateActor<SampleAggregate>(_aggregateId);

            aggregateActorRef.Tell(new NotifyOnPersistenceEvents(TestActor), TestActor);

            await Task.WhenAll(ChangeSeveralTimes(5));

            Watch(aggregateActorRef);
            aggregateActorRef.Tell(GracefullShutdownRequest.Instance, TestActor);

            FishForMessage<Terminated>(m => true, TimeSpan.FromMinutes(10));

            _snapshots = new AggregateSnapshotRepository(AkkaConf.Persistence.JournalConnectionString, GridNode.AggregateFromSnapshotsFactory).Load<SampleAggregate>(_aggregateId);
        }

        private IEnumerable<Task> ChangeSeveralTimes(int changeNumber)
        {
            for (var cmdNum = 0; cmdNum < changeNumber; cmdNum++)
            {
                _parameters[cmdNum] = cmdNum;
                var changeCmd = new ChangeSampleAggregateCommand(cmdNum, _aggregateId);
                yield return GridNode.Prepare(changeCmd)
                                     .Expect<SampleAggregateChangedEvent>(e => e.Value == changeCmd.Parameter.ToString())
                                     .Execute();
            }
        }

        [Test]
        public void Only_2_Snapshots_should_left()
        {
            Assert.AreEqual(2, _snapshots.Length);
        }

        [Test]
        public void Restored_aggregates_should_have_same_ids()
        {
            Assert.True(_snapshots.All(s => s.Aggregate.Id == _aggregateId));
        }

        [Test]
        public void Snapshots_should_have_parameters_from_last_command()
        {
            CollectionAssert.AreEqual(_parameters.Skip(3).Take(2).Select(p => p.ToString()),
                _snapshots.Select(s => s.Aggregate.Value));
        }

        [Test]
        public void All_snapshots_should_not_have_uncommited_events()
        {
            CollectionAssert.IsEmpty(_snapshots.SelectMany(s => s.Aggregate.GetEvents()));
        }
    }
}