using System;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.Tests.Common;
using GridDomain.Tests.Unit.BalloonDomain.Commands;
using GridDomain.Tests.Unit.BalloonDomain.Events;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.CommandsExecution
{
    public class When_execute_command_without_explicit_timeout_Then_wait_should_use_default_timeout :
        SampleDomainCommandExecutionTests
    {
        public When_execute_command_without_explicit_timeout_Then_wait_should_use_default_timeout(ITestOutputHelper output)
            : base(output) {}

        [Fact]
        public async Task PlanExecute_by_result_throws_exception_after_default_timeout()
        {
            var syncCommand = new PlanTitleWriteCommand(1000, Guid.NewGuid());

            await
                Node.Prepare(syncCommand)
                    .Expect<BalloonTitleChanged>()
                    .Execute(TimeSpan.FromMilliseconds(500))
                    .ShouldThrow<TimeoutException>();
        }
    }
}