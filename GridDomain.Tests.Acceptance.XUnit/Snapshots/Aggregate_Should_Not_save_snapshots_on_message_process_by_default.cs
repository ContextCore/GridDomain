using System;
using System.Threading;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.Tests.XUnit;
using GridDomain.Tests.XUnit.CommandsExecution;
using GridDomain.Tests.XUnit.SampleDomain;
using GridDomain.Tests.XUnit.SampleDomain.Commands;
using GridDomain.Tests.XUnit.SampleDomain.Events;
using GridDomain.Tools.Repositories.AggregateRepositories;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Acceptance.XUnit.Snapshots
{
    public class Aggregate_Should_Not_save_snapshots_on_message_process_by_default : NodeTestKit
    {
        [Fact]
        public async Task Given_timeout_only_default_policy()
        {
            var aggregateId = Guid.NewGuid();

            var cmd = new CreateSampleAggregateCommand(1, aggregateId);
            await Node.Prepare(cmd)
                      .Expect<SampleAggregateCreatedEvent>()
                      .Execute();

            //checking "time-out" rule for policy, snapshots should be saved once on second command
            await Task.Delay(1000);

            var changedParameter = 2;
            var changeSampleAggregateCommand = new ChangeSampleAggregateCommand(changedParameter, aggregateId);

            await Node.Prepare(changeSampleAggregateCommand)
                      .Expect<SampleAggregateChangedEvent>()
                      .Execute();

            await Task.Delay(1000);

            var snapshots =
                await
                    new AggregateSnapshotRepository(AkkaConfig.Persistence.JournalConnectionString,
                        Node.AggregateFromSnapshotsFactory).Load<SampleAggregate>(aggregateId);

            //Snapshots_should_be_saved_one_time()
            Assert.Equal(0, snapshots.Length);
        }

        public Aggregate_Should_Not_save_snapshots_on_message_process_by_default(ITestOutputHelper output) : base(output, 
            new SampleDomainFixture() {InMemory = false}) {}
    }
}