using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Akka.Actor;
using Akka.Persistence;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.Node.Actors;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Tests.CommandsExecution;
using GridDomain.Tests.Framework;
using GridDomain.Tests.SampleDomain;
using GridDomain.Tests.SampleDomain.Commands;
using GridDomain.Tests.SampleDomain.Events;
using GridDomain.Tools.Repositories;
using GridDomain.Tools.Repositories.AggregateRepositories;
using NUnit.Framework;
using Ploeh.AutoFixture;

namespace GridDomain.Tests.Acceptance.Snapshots
{
    [TestFixture]
    class Aggregate_Should_delete_snapshots_according_to_policy_on_shutdown: SampleDomainCommandExecutionTests
    {
        private Guid _aggregateId;
        private AggregateVersion<SampleAggregate>[] _snapshots;
        private readonly int[] _parameters = new int[5];

        public Aggregate_Should_delete_snapshots_according_to_policy_on_shutdown():base(false)
        {
            
        }

        protected override IContainerConfiguration CreateConfiguration()
        {
            return new CustomContainerConfiguration(
                base.CreateConfiguration(),
                new AggregateConfiguration<SampleAggregate, SampleAggregatesCommandHandler>(
                                                          () => new SnapshotsSaveAfterEachMessagePolicy(2),
                                                          SampleAggregate.FromSnapshot
                                                          )
                );
        }

       // protected override TimeSpan Timeout { get; } //= TimeSpan.FromMinutes(10);

        [OneTimeSetUp]
        public void Given_save_on_each_message_policy_and_keep_2_snapshots()
        {
            _aggregateId = Guid.NewGuid();
            var cmd = new CreateSampleAggregateCommand(1,_aggregateId);

            GridNode.NewCommandWaiter(Timeout)
                    .Expect<SampleAggregateCreatedEvent>()
                    .Create()
                    .Execute(cmd)
                    .Wait();   
              
            var aggregateActorRef = LookupAggregateActor<SampleAggregate>(_aggregateId);

            aggregateActorRef.Tell(new NotifyOnPersistenceEvents(TestActor),TestActor);

            var commands = new List<ICommand> {cmd};

            var waiter = GridNode.NewCommandWaiter(Timeout)
                                 .Expect<SampleAggregateCreatedEvent>();


            for (var cmdNum = 0; cmdNum < 5; cmdNum ++)
            {
                var changeCmd = new ChangeSampleAggregateCommand(cmdNum, _aggregateId);
                waiter.And<SampleAggregateChangedEvent>(e => e.Value == changeCmd.Parameter.ToString());
                commands.Add(changeCmd);
                _parameters[cmdNum] = cmdNum;
            }

            waiter.Create()
                  .Execute(commands.ToArray())
                  .Wait();

            Watch(aggregateActorRef);
            aggregateActorRef.Tell(GracefullShutdownRequest.Instance, TestActor);

            FishForMessage<Terminated>(m => true,TimeSpan.FromMinutes(10));

            _snapshots = new AggregateSnapshotRepository(AkkaConf.Persistence.JournalConnectionString, GridNode.AggregateFromSnapshotsFactory).Load<SampleAggregate>(_aggregateId);
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