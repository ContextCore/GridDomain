using System;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.Tests.Unit.BalloonDomain;
using GridDomain.Tests.Unit.BalloonDomain.Commands;
using GridDomain.Tests.Unit.BalloonDomain.Events;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.CommandsExecution
{
    public class When_execute_command_Then_aggregate_Should_persist_changed : SampleDomainCommandExecutionTests
    {
        public When_execute_command_Then_aggregate_Should_persist_changed(ITestOutputHelper output) : base(output) {}

        [Fact]
        public async Task Async_method_should_change_aggregate()
        {
            var syncCommand = new PlanTitleChangeCommand(42, Guid.NewGuid());

            await Node.Prepare(syncCommand).Expect<BalloonTitleChanged>().Execute();

            //to finish persistence
            var aggregate = await this.LoadAggregate<Balloon>(syncCommand.AggregateId);
            Assert.Equal(syncCommand.Parameter.ToString(), aggregate.Title);
        }

        [Fact]
        public async Task Sync_method_should_change_aggregate()
        {
            var syncCommand = new PlanTitleWriteCommand(42, Guid.NewGuid());

            await Node.Prepare(syncCommand).Expect<BalloonTitleChanged>().Execute();

            //to finish persistence
            var aggregate = await this.LoadAggregate<Balloon>(syncCommand.AggregateId);
            Assert.Equal(syncCommand.Parameter.ToString(), aggregate.Title);
        }
    }
}