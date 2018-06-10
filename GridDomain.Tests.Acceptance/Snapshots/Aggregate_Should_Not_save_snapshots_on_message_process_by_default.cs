using System;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.Tests.Unit;
using GridDomain.Tests.Unit.BalloonDomain;
using GridDomain.Tests.Unit.BalloonDomain.Commands;
using GridDomain.Tests.Unit.BalloonDomain.Events;
using GridDomain.Tests.Unit.ProcessManagers;
using GridDomain.Tools.Repositories.AggregateRepositories;
using Serilog.Events;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Acceptance.Snapshots
{
    public class Aggregate_Should_Not_save_snapshots_on_message_process_by_default : NodeTestKit
    {
        protected Aggregate_Should_Not_save_snapshots_on_message_process_by_default(NodeTestFixture fixture) : base(fixture) { }

        public Aggregate_Should_Not_save_snapshots_on_message_process_by_default(ITestOutputHelper output)
            : this(new BalloonFixture(output).UseSqlPersistence()) { }

        [Fact]
        public async Task Given_timeout_only_default_policy()
        {
            var aggregateId = Guid.NewGuid()
                                  .ToString();

            var cmd = new InflateNewBallonCommand(1, aggregateId);
            await Node.Prepare(cmd)
                      .Expect<BalloonCreated>()
                      .Execute();

            //checking "time-out" rule for policy, snapshots should be saved once on second command
            await Task.Delay(1000);

            var changedParameter = 2;
            var changeSampleAggregateCommand = new WriteTitleCommand(changedParameter, aggregateId);

            await Node.Prepare(changeSampleAggregateCommand)
                      .Expect<BalloonTitleChanged>()
                      .Execute();

            await Task.Delay(1000);

            var snapshots =
                await
                    new AggregateSnapshotRepository(AutoTestNodeDbConfiguration.Default.JournalConnectionString,
                                                    new BalloonAggregateFactory(),
                                                    new BalloonAggregateFactory()).Load<Balloon>(aggregateId);

            //Snapshots_should_be_saved_one_time()
            Assert.Empty(snapshots);
        }
    }
}