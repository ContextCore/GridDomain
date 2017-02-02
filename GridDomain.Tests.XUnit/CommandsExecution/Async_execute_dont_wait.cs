using System;
using System.Threading.Tasks;
using GridDomain.Tests.XUnit.SampleDomain;
using GridDomain.Tests.XUnit.SampleDomain.Commands;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.XUnit.CommandsExecution
{
  
    public class Async_execute_dont_wait : SampleDomainCommandExecutionTests
    {
       [Fact]
        public async Task Async_execute_dont_wait_for_command_finish()
        {
            var syncCommand = new AsyncMethodCommand(42, Guid.NewGuid());
            Node.Execute(syncCommand);
            var aggregate = await this.LoadAggregate<SampleAggregate>(syncCommand.AggregateId);
            Assert.NotEqual(syncCommand.Parameter.ToString(), aggregate.Value);
        }

        public Async_execute_dont_wait(ITestOutputHelper output) : base(output)
        {
        }
    }
}