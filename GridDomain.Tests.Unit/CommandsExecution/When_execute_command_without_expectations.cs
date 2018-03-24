using System;
using System.Threading.Tasks;
using GridDomain.Tests.Unit.BalloonDomain;
using GridDomain.Tests.Unit.BalloonDomain.Commands;
using GridDomain.Tests.Unit.CommandsExecution.ExecutionWithErrors;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.CommandsExecution
{
    public class When_execute_command_without_expectations : BalloonDomainCommandExecutionTests
    {
        public When_execute_command_without_expectations(ITestOutputHelper output) : base(output) {}

        [Fact]
        public async Task Aggregate_will_apply_events_later_than_command_execution_finish()
        {
            var syncCommand = new PlanTitleWriteCommand(42, Guid.NewGuid());
#pragma warning disable 4014
            Node.Execute(syncCommand);
#pragma warning restore 4014
            var aggregate = await this.LoadAggregateByActor<Balloon>(syncCommand.AggregateId);
            Assert.NotEqual(syncCommand.Parameter.ToString(), aggregate.Title);
        }
    }
}