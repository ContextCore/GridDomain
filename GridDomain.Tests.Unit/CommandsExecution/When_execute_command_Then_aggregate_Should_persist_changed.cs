using System;
using System.Threading.Tasks;
using GridDomain.Configuration;
using GridDomain.CQRS;
using GridDomain.Tests.Unit.BalloonDomain;
using GridDomain.Tests.Unit.BalloonDomain.Commands;
using GridDomain.Tests.Unit.BalloonDomain.Configuration;
using GridDomain.Tests.Unit.BalloonDomain.Events;
using GridDomain.Tests.Unit.CommandsExecution.ExecutionWithErrors;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.CommandsExecution
{
    public class When_execute_command_Then_aggregate_Should_persist_changed : NodeTestKit
    {
        public When_execute_command_Then_aggregate_Should_persist_changed(ITestOutputHelper output) : this(new NodeTestFixture(output)) {}
        protected When_execute_command_Then_aggregate_Should_persist_changed(NodeTestFixture output) : base(output.Add(new BalloonDomainConfiguration())) {}

        [Fact]
        public async Task Async_method_should_change_aggregate()
        {
            var syncCommand = new PlanTitleChangeCommand(Guid.NewGuid().ToString(),42);

            await Node.Prepare(syncCommand).Expect<BalloonTitleChanged>().Execute();

            //to finish persistence
            var aggregate = await Node.LoadAggregateByActor<Balloon>(syncCommand.AggregateId);
            Assert.Equal(syncCommand.Parameter.ToString(), aggregate.Title);
        }

        [Fact]
        public async Task Sync_method_should_change_aggregate()
        {
            var syncCommand = new PlanTitleWriteCommand(42, Guid.NewGuid());

            await Node.Prepare(syncCommand).Expect<BalloonTitleChanged>().Execute();

            //to finish persistence
            var aggregate = await Node.LoadAggregateByActor<Balloon>(syncCommand.AggregateId);
            Assert.Equal(syncCommand.Parameter.ToString(), aggregate.Title);
        }
    }
}