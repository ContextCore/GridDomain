using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.Tests.Acceptance.EventsUpgrade;
using GridDomain.Tests.Common;
using GridDomain.Tests.Unit;
using GridDomain.Tests.Unit.BalloonDomain;
using GridDomain.Tests.Unit.BalloonDomain.Commands;
using GridDomain.Tests.Unit.BalloonDomain.Events;
using GridDomain.Tools.Repositories.AggregateRepositories;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Acceptance.Snapshots
{
    public class Aggregate_Should_save_snapshots_after_each_message_according_to_save_policy : NodeTestKit
    {
        public Aggregate_Should_save_snapshots_after_each_message_according_to_save_policy(ITestOutputHelper output)
            : base(output, new BalloonFixture {InMemory = false}.EnableSnapshots()) {}

        [Fact]
        public async Task Given_default_policy()
        {
            var aggregateId = Guid.NewGuid();
            var initialParameter = 1;
            var cmd = new InflateNewBallonCommand(initialParameter, aggregateId);
            await Node.Prepare(cmd)
                      .Expect<BalloonCreated>()
                      .Execute();

            var changedParameter = 2;
            var changeSampleAggregateCommand = new WriteTitleCommand(changedParameter, aggregateId);

            await Node.Prepare(changeSampleAggregateCommand)
                      .Expect<BalloonTitleChanged>()
                      .Execute();

            Thread.Sleep(100);

            var snapshots = await new AggregateSnapshotRepository(AkkaConfig.Persistence.JournalConnectionString,
                                                                  new BalloonAggregateFactory())
                                                                  .Load<Balloon>(aggregateId);
            //Snapshots_should_be_saved_two_times()
            Assert.Equal(2, snapshots.Length);
            //Restored_aggregates_should_have_same_ids()
            Assert.True(snapshots.All(s => s.Aggregate.Id == aggregateId));
            //First_snapshot_should_have_parameters_from_first_command()
            Assert.Equal(initialParameter.ToString(), snapshots.First().Aggregate.Title);
            //Second_snapshot_should_have_parameters_from_second_command()
            Assert.Equal(changedParameter.ToString(), snapshots.Skip(1).First().Aggregate.Title);
            //All_snapshots_should_not_have_uncommited_events()
            Assert.Empty(snapshots.SelectMany(s => s.Aggregate.GetEvents()));
        }
    }
}