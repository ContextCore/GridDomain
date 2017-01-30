using System;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.Tests.Framework;
using GridDomain.Tests.XUnit.SampleDomain.Commands;
using GridDomain.Tests.XUnit.SampleDomain.Events;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.XUnit.CommandsExecution
{
  
    public class When_execute_command_without_explicit_timeout_Then_wait_should_use_default_timeout : SampleDomainCommandExecutionTests
    {
        public When_execute_command_without_explicit_timeout_Then_wait_should_use_default_timeout(ITestOutputHelper output) : base(output)
        {
        }

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