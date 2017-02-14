using System;
using System.Threading.Tasks;
using GridDomain.Common;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Tests.Framework;
using GridDomain.Tests.XUnit;
using GridDomain.Tests.XUnit.CommandsExecution;
using GridDomain.Tests.XUnit.SampleDomain;
using GridDomain.Tools.Repositories.AggregateRepositories;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Acceptance.XUnit.Snapshots
{
    public class Aggregate_should_recover_from_snapshot : NodeTestKit
    {
        class RecoverFixture : SampleDomainFixture
        {
            public RecoverFixture()
            {
                Add(
                    new CustomContainerConfiguration(
                        new AggregateConfiguration<SampleAggregate, SampleAggregatesCommandHandler>(
                            () => new SnapshotsPersistenceAfterEachMessagePolicy(),
                            SampleAggregate.FromSnapshot)));
            }
        }

        [Fact]
        public async Task Test()
        {
            var aggregate = new SampleAggregate(Guid.NewGuid(), "test");
            aggregate.ChangeState(10);
            aggregate.ClearEvents();

            var repo = new AggregateSnapshotRepository(Fixture.AkkaConfig.Persistence.JournalConnectionString,
                Node.AggregateFromSnapshotsFactory);
            await repo.Add(aggregate);

            var restoredAggregate = await Node.LoadAggregate<SampleAggregate>(aggregate.Id);
            //Values_should_be_equal()
            Assert.Equal(aggregate.Value, restoredAggregate.Value);
            //State_restored_from_snapshot_should_not_have_uncommited_events()
            Assert.Empty(restoredAggregate.GetEvents());
            //Ids_should_be_equal()
            Assert.Equal(aggregate.Id, restoredAggregate.Id);
        }

        public Aggregate_should_recover_from_snapshot(ITestOutputHelper output) : base(output, new RecoverFixture()) {}
    }
}