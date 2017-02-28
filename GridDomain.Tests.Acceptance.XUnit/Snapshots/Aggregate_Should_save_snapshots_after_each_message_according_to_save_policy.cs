using System;
using System.Linq;
using System.Threading;
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
    public class Aggregate_Should_save_snapshots_after_each_message_according_to_save_policy : NodeTestKit
    {
        public Aggregate_Should_save_snapshots_after_each_message_according_to_save_policy(ITestOutputHelper output)
            : base(output, new SampleDomainFixture {InMemory = false}.InitSampleAggregateSnapshots()) {}

        [Fact]
        public async Task Given_default_policy()
        {
            var aggregateId = Guid.NewGuid();
            var initialParameter = 1;
            var cmd = new CreateSampleAggregateCommand(initialParameter, aggregateId);
            await Node.Prepare(cmd)
                      .Expect<SampleAggregateCreatedEvent>()
                      .Execute();

            var changedParameter = 2;
            var changeSampleAggregateCommand = new ChangeSampleAggregateCommand(changedParameter, aggregateId);

            await Node.Prepare(changeSampleAggregateCommand)
                      .Expect<SampleAggregateChangedEvent>()
                      .Execute();

            Thread.Sleep(100);

            var snapshots =
                await
                    new AggregateSnapshotRepository(AkkaConfig.Persistence.JournalConnectionString,
                        Node.AggregateFromSnapshotsFactory).Load<SampleAggregate>(aggregateId);
            //Snapshots_should_be_saved_two_times()
            Assert.Equal(2, snapshots.Length);
            //Restored_aggregates_should_have_same_ids()
            Assert.True(snapshots.All(s => s.Aggregate.Id == aggregateId));
            //First_snapshot_should_have_parameters_from_first_command()
            Assert.Equal(initialParameter.ToString(),
                snapshots.First()
                         .Aggregate.Value);
            //Second_snapshot_should_have_parameters_from_second_command()
            Assert.Equal(changedParameter.ToString(),
                snapshots.Skip(1)
                         .First()
                         .Aggregate.Value);
            //All_snapshots_should_not_have_uncommited_events()
            Assert.Empty(snapshots.SelectMany(s => s.Aggregate.GetEvents()));
        }
    }
}