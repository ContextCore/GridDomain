using System;
using System.Threading.Tasks;
using GridDomain.Tests.Unit.BalloonDomain;
using GridDomain.Tests.Unit.BalloonDomain.Commands;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.CommandsExecution
{
    public class Async_execute_dont_wait : BalloonDomainCommandExecutionTests
    {
        public Async_execute_dont_wait(ITestOutputHelper output) : base(output) {}

        [Fact]
        public async Task Async_execute_dont_wait_for_command_finish()
        {
            var syncCommand = new PlanTitleChangeCommand(42, Guid.NewGuid().ToString());
            //intentionally dont wait for command execution finish
#pragma warning disable 4014
            Node.Execute(syncCommand);
#pragma warning restore 4014
            var aggregate = await this.LoadAggregateByActor<Balloon>(syncCommand.AggregateId);
            Assert.NotEqual(syncCommand.Parameter.ToString(), aggregate.Title);
        }
    }
}