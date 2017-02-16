using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.Node.Actors;
using GridDomain.Node.Configuration.Akka;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Tests.Acceptance.XUnit.EventsUpgrade;
using GridDomain.Tests.Framework;
using GridDomain.Tests.XUnit;
using GridDomain.Tests.XUnit.CommandsExecution;
using GridDomain.Tests.XUnit.SampleDomain;
using GridDomain.Tests.XUnit.SampleDomain.Commands;
using GridDomain.Tests.XUnit.SampleDomain.Events;
using GridDomain.Tools.Repositories.AggregateRepositories;
using Serilog.Events;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Acceptance.XUnit.Snapshots
{
    public class Aggregate_Should_delete_snapshots_according_to_policy_on_shutdown : NodeTestKit
    {
        private readonly int[] _parameters = new int[5];

        [Fact]
        public async Task Given_save_on_each_message_policy_and_keep_2_snapshots()
        {
            var aggregateId = Guid.NewGuid();
            var cmd = new CreateSampleAggregateCommand(1, aggregateId);

            await Node.Prepare(cmd)
                      .Expect<SampleAggregateCreatedEvent>()
                      .Execute();

            var aggregateActorRef = await Node.System.LookupAggregateActor<SampleAggregate>(aggregateId);

            aggregateActorRef.Tell(new NotifyOnPersistenceEvents(TestActor), TestActor);

            await Task.WhenAll(ChangeSeveralTimes(5, aggregateId));

            Watch(aggregateActorRef);
            aggregateActorRef.Tell(GracefullShutdownRequest.Instance, TestActor);

            FishForMessage<Terminated>(m => true,DefaultTimeOut);

            var snapshots =
                await
                    new AggregateSnapshotRepository(AkkaConfig.Persistence.JournalConnectionString,
                        Node.AggregateFromSnapshotsFactory).Load<SampleAggregate>(aggregateId);

            //Only_2_Snapshots_should_left()
            Assert.Equal(2, snapshots.Length);
            //Restored_aggregates_should_have_same_ids()
            Assert.True(snapshots.All(s => s.Aggregate.Id == aggregateId));
            //Snapshots_should_have_parameters_from_last_command()
            Assert.Equal(_parameters.Skip(3)
                                    .Take(2)
                                    .Select(p => p.ToString()),
                snapshots.Select(s => s.Aggregate.Value));
            //All_snapshots_should_not_have_uncommited_events()
            Assert.Empty(snapshots.SelectMany(s => s.Aggregate.GetEvents()));
        }

        private IEnumerable<Task> ChangeSeveralTimes(int changeNumber, Guid aggregateId)
        {
            for (var cmdNum = 0; cmdNum < changeNumber; cmdNum++)
            {
                _parameters[cmdNum] = cmdNum;
                var changeCmd = new ChangeSampleAggregateCommand(cmdNum, aggregateId);
                yield return Node.Prepare(changeCmd)
                                 .Expect<SampleAggregateChangedEvent>(e => e.Value == changeCmd.Parameter.ToString())
                                 .Execute();
            }
        }

        public Aggregate_Should_delete_snapshots_according_to_policy_on_shutdown(ITestOutputHelper output)
            : base(output, new SampleDomainFixture {InMemory = false}.InitSampleAggregateSnapshots(2)) {}

      
    }
}