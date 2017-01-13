using System;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.Tests.Unit.SampleDomain.Commands;
using GridDomain.Tests.Unit.SampleDomain.Events;
using NUnit.Framework;

namespace GridDomain.Tests.Unit.CommandsExecution
{
    [TestFixture]
    public class When_execute_command_without_explicit_timeout_Then_wait_should_use_default_timeout : SampleDomainCommandExecutionTests
    {
        [Then]
        public async Task PlanExecute_by_result_throws_exception_after_default_timeout()
        {
            var syncCommand = new LongOperationCommand(1000, Guid.NewGuid());

            await GridNode.Prepare(syncCommand)
                          .Expect<SampleAggregateChangedEvent>()
                          .Execute(TimeSpan.FromMilliseconds(500))
                          .ShouldThrow<TimeoutException>();

        }
    }
}