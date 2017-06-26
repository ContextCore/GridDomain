using System;
using System.Threading.Tasks;
using GridDomain.Tests.XUnit.BalloonDomain;
using GridDomain.Tests.XUnit.BalloonDomain.Commands;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.XUnit.CommandsExecution
{
    public class Async_execute_dont_wait : SampleDomainCommandExecutionTests
    {
        public Async_execute_dont_wait(ITestOutputHelper output) : base(output) {}

        [Fact]
        public async Task Async_execute_dont_wait_for_command_finish()
        {
            var syncCommand = new PlanTitleChangeCommand(42, Guid.NewGuid());
            Node.Execute(syncCommand);
            var aggregate = await this.LoadAggregate<Balloon>(syncCommand.AggregateId);
            Assert.NotEqual(syncCommand.Parameter.ToString(), aggregate.Title);
        }
    }
}