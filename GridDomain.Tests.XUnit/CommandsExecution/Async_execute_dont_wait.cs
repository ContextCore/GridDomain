using System;
using GridDomain.Tests.XUnit.SampleDomain;
using GridDomain.Tests.XUnit.SampleDomain.Commands;
using Xunit;

namespace GridDomain.Tests.XUnit.CommandsExecution
{
  
    public class Async_execute_dont_wait : SampleDomainCommandExecutionTests
    {
       [Fact]
        public void Async_execute_dont_wait_for_command_finish()
        {
            var syncCommand = new AsyncMethodCommand(42, Guid.NewGuid());
            GridNode.Execute(syncCommand);
            var aggregate = this.LoadAggregate<SampleAggregate>(syncCommand.AggregateId);
            Assert.NotEqual(syncCommand.Parameter, aggregate.Value);
        }
    }
}