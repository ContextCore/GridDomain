using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.Tests.Acceptance.XUnit.EventsUpgrade;
using GridDomain.Tests.Framework;
using GridDomain.Tests.XUnit;
using GridDomain.Tests.XUnit.SampleDomain;
using GridDomain.Tests.XUnit.SampleDomain.Commands;
using GridDomain.Tests.XUnit.SampleDomain.Events;
using GridDomain.Tools.Repositories.AggregateRepositories;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Acceptance.XUnit.Snapshots
{
    public class Aggregate_Should_delete_snapshots_according_to_policy_on_shutdown : NodeTestKit
    {
        public Aggregate_Should_delete_snapshots_according_to_policy_on_shutdown(ITestOutputHelper output)
            : base(output, new SampleDomainFixture {InMemory = false}.InitSampleAggregateSnapshots(2)) {}

        private readonly int[] _parameters = new int[5];

        private IEnumerable<Task> ChangeSeveralTimes(int changeNumber, Guid aggregateId)
        {
            for (var cmdNum = 0; cmdNum < changeNumber; cmdNum++)
            {
                _parameters[cmdNum] = cmdNum;
                yield return
                    Node.Prepare(new ChangeSampleAggregateCommand(cmdNum, aggregateId))
                        .Expect<SampleAggregateChangedEvent>()
                        .Execute();
            }
        }

        [Fact]
        public async Task Given_save_on_each_message_policy_and_keep_2_snapshots()
        {
            var aggregateId = Guid.NewGuid();

            await
                Node.Prepare(new CreateSampleAggregateCommand(1, aggregateId))
                    .Expect<SampleAggregateCreatedEvent>()
                    .Execute();

            await Task.WhenAll(ChangeSeveralTimes(5, aggregateId));

            await Task.Delay(5000);

            await Node.KillAggregate<SampleAggregate>(aggregateId);

            var snapshots =
                await
                    new AggregateSnapshotRepository(AkkaConfig.Persistence.JournalConnectionString,
                        Node.AggregateFromSnapshotsFactory).Load<SampleAggregate>(aggregateId);

            //Only_2_Snapshots_should_left()
            Assert.Equal(2, snapshots.Length);
            //Restored_aggregates_should_have_same_ids()
            Assert.True(snapshots.All(s => s.Aggregate.Id == aggregateId));
            //Snapshots_should_have_parameters_from_last_command()
            Assert.Equal(_parameters.Skip(3).Take(2).Select(p => p.ToString()), snapshots.Select(s => s.Aggregate.Value));
            //All_snapshots_should_not_have_uncommited_events()
            Assert.Empty(snapshots.SelectMany(s => s.Aggregate.GetEvents()));
        }
    }
}