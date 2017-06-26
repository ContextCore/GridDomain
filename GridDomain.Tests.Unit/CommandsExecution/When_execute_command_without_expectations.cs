using System;
using System.Threading.Tasks;
using GridDomain.Tests.Unit.BalloonDomain;
using GridDomain.Tests.Unit.BalloonDomain.Commands;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.CommandsExecution
{
    public class When_execute_command_without_expectations : SampleDomainCommandExecutionTests
    {
        public When_execute_command_without_expectations(ITestOutputHelper output) : base(output) {}

        [Fact]
        public async Task Aggregate_will_apply_events_later_than_command_execution_finish()
        {
            var syncCommand = new PlanTitleWriteCommand(42, Guid.NewGuid());
            Node.Execute(syncCommand);
            var aggregate = await this.LoadAggregate<Balloon>(syncCommand.AggregateId);
            Assert.NotEqual(syncCommand.Parameter.ToString(), aggregate.Title);
        }
    }
}