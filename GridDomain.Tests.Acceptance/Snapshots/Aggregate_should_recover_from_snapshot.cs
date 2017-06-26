using System;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.EventSourcing;
using GridDomain.Tests.Unit;
using GridDomain.Tests.Unit.BalloonDomain;
using GridDomain.Tests.Unit.BalloonDomain.Events;
using GridDomain.Tools.Repositories.AggregateRepositories;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Acceptance.Snapshots
{
    public class Aggregate_should_recover_from_snapshot : NodeTestKit
    {
        public Aggregate_should_recover_from_snapshot(ITestOutputHelper output)
            : base(output, new BalloonFixture {InMemory = false}.InitSampleAggregateSnapshots()) {}

        [Fact]
        public async Task Given_persisted_snapshot_Aggregate_should_execute_command()
        {
            var aggregate = new Balloon(Guid.NewGuid(), "haha");
            aggregate.WriteNewTitle(10);
            aggregate.PersistAll();

            var repo = new AggregateSnapshotRepository(AkkaConfig.Persistence.JournalConnectionString,
                                                       Node.AggregateFromSnapshotsFactory);
            await repo.Add(aggregate);

            var cmd = new IncreaseTitleCommand(1, aggregate.Id);

            var res = await Node.Prepare(cmd)
                                .Expect<BalloonTitleChanged>()
                                .Execute();

            var message = res.Received;

            //Values_should_be_equal()
            Assert.Equal("11", message.Value);
            //Ids_should_be_equal()
            Assert.Equal(aggregate.Id, message.SourceId);
        }
    }
}