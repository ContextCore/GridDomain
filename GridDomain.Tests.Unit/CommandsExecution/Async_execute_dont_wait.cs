using System;
using System.Threading.Tasks;
using GridDomain.Tests.Unit.BalloonDomain;
using GridDomain.Tests.Unit.BalloonDomain.Commands;
using GridDomain.Tests.Unit.BalloonDomain.Configuration;
using GridDomain.Tests.Unit.CommandsExecution.ExecutionWithErrors;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.CommandsExecution
{
    public class Async_execute_dont_wait : NodeTestKit
    {
        public Async_execute_dont_wait(ITestOutputHelper output) : this(new NodeTestFixture(output)) {}
        protected Async_execute_dont_wait(NodeTestFixture fixture) : base(fixture.Add(new BalloonDomainConfiguration())) {}

        [Fact]
        public async Task Async_execute_dont_wait_for_command_finish()
        {
            var syncCommand = new PlanTitleChangeCommand(Guid.NewGuid().ToString(),42);
            //intentionally dont wait for command execution finish
#pragma warning disable 4014
            Node.Execute(syncCommand);
#pragma warning restore 4014
            var aggregate = await this.LoadAggregateByActor<Balloon>(syncCommand.AggregateId);
            Assert.NotEqual(syncCommand.Parameter.ToString(), aggregate.Title);
        }
    }
}