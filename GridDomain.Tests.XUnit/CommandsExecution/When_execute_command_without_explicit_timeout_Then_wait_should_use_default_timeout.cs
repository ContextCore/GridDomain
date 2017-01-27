using System;
using System.Threading.Tasks;
using GridDomain.CQRS;

namespace GridDomain.Tests.XUnit.CommandsExecution
{
  
    public class When_execute_command_without_explicit_timeout_Then_wait_should_use_default_timeout : SampleDomainCommandExecutionTests
    {
       [Fact]
        public async Task PlanExecute_by_result_throws_exception_after_default_timeout()
        {
            var syncCommand = new LongOperationCommand(1000, Guid.NewGuid());

            await Node.Prepare(syncCommand)
                          .Expect<SampleAggregateChangedEvent>()
                          .Execute(TimeSpan.FromMilliseconds(500))
                          .ShouldThrow<TimeoutException>();

        }
    }
}