using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.Tests.Acceptance.EventsUpgrade;
using GridDomain.Tests.Acceptance.Tools;
using GridDomain.Tests.Common;
using GridDomain.Tests.Unit;
using GridDomain.Tests.Unit.BalloonDomain;
using GridDomain.Tests.Unit.BalloonDomain.Commands;
using GridDomain.Tests.Unit.BalloonDomain.Events;
using GridDomain.Tests.Unit.ProcessManagers;
using GridDomain.Tools.Repositories.AggregateRepositories;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Acceptance.Snapshots
{
    public class Aggregate_Should_save_snapshots_after_each_message_according_to_save_policy : NodeTestKit
    {
        public Aggregate_Should_save_snapshots_after_each_message_according_to_save_policy(ITestOutputHelper output)
            : base(new BalloonFixture(output).UseSqlPersistence().EnableSnapshots()) {}

        [Fact]
        public async Task Given_default_policy()
        {
            var aggregateId = Guid.NewGuid().ToString();
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

            var aggregates = await AggregateSnapshotRepository.New(AutoTestNodeDbConfiguration.Default.JournalConnectionString,
                                                                   BalloonAggregateFactory.Default)
                                                                  .Load<Balloon>(aggregateId);
            //Snapshots_should_be_saved_two_times()
            Assert.Equal(2, aggregates.Length);
            //Restored_aggregates_should_have_same_ids()
            Assert.True(aggregates.All(s => s.Payload.Id == aggregateId));
            //First_snapshot_should_have_parameters_from_first_command()
            Assert.Equal(initialParameter.ToString(), aggregates.First().Payload.Title);
            //Second_snapshot_should_have_parameters_from_second_command()
            Assert.Equal(changedParameter.ToString(), aggregates.Skip(1).First().Payload.Title);
        }
    }
}