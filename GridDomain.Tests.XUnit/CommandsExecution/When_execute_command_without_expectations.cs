using System;
using System.Threading.Tasks;
using GridDomain.Tests.XUnit.SampleDomain;
using GridDomain.Tests.XUnit.SampleDomain.Commands;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.XUnit.CommandsExecution
{
  
    public class When_execute_command_without_expectations : SampleDomainCommandExecutionTests
    {
       [Fact]
        public async Task Aggregate_will_apply_events_later_than_command_execution_finish()
        {
            var syncCommand = new LongOperationCommand(42, Guid.NewGuid());
            Node.Execute(syncCommand);
            var aggregate = await this.LoadAggregate<SampleAggregate>(syncCommand.AggregateId);
            Assert.NotEqual(syncCommand.Parameter.ToString(), aggregate.Value);
        }

        public When_execute_command_without_expectations(ITestOutputHelper output) : base(output)
        {
        }
    }
}