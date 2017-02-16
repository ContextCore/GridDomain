using System;
using System.Threading.Tasks;
using GridDomain.Common;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Tests.Acceptance.XUnit.EventsUpgrade;
using GridDomain.Tests.Framework;
using GridDomain.Tests.XUnit;
using GridDomain.Tests.XUnit.CommandsExecution;
using GridDomain.Tests.XUnit.SampleDomain;
using GridDomain.Tests.XUnit.SampleDomain.Commands;
using GridDomain.Tools.Repositories.AggregateRepositories;
using Xunit;
using Xunit.Abstractions;
using GridDomain.CQRS;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Tests.XUnit.SampleDomain.Events;

namespace GridDomain.Tests.Acceptance.XUnit.Snapshots
{
    public class Aggregate_should_recover_from_snapshot : NodeTestKit
    {

        [Fact]
        public async Task Test()
        {
            var aggregate = new SampleAggregate(Guid.NewGuid(), "test");
            aggregate.ChangeState(10);
            aggregate.ClearEvents();

            var repo = new AggregateSnapshotRepository(AkkaConfig.Persistence.JournalConnectionString,Node.AggregateFromSnapshotsFactory);
            await repo.Add(aggregate);

            var cmd = new IncreaseSampleAggregateCommand(1, aggregate.Id);

            var res = await Node.Prepare(cmd)
                                .Expect<SampleAggregateChangedEvent>()
                                .Execute();

            var message = res.Message<SampleAggregateChangedEvent>();

            //Values_should_be_equal()
            Assert.Equal("11", message.Value);
            //Ids_should_be_equal()
            Assert.Equal(aggregate.Id, message.SourceId);
        }

        public Aggregate_should_recover_from_snapshot(ITestOutputHelper output) : base(output,
            new SampleDomainFixture { InMemory = false }.InitSampleAggregateSnapshots()) {}
    }
}