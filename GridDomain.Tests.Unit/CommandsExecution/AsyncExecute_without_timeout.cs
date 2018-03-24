using System;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.Tests.Unit.BalloonDomain.Commands;
using GridDomain.Tests.Unit.BalloonDomain.Events;
using GridDomain.Tests.Unit.CommandsExecution.ExecutionWithErrors;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.CommandsExecution
{
    public class AsyncExecute_without_timeout : BalloonDomainCommandExecutionTests
    {
        public AsyncExecute_without_timeout(ITestOutputHelper output) : base(output) {}

        [Fact]
        public async Task CommandWaiter_throws_exception_after_wait_with_only_default_timeout()
        {
            var syncCommand = new PlanTitleWriteCommand(1000, Guid.NewGuid().ToString());
            var waiter =
                Node.Prepare(syncCommand)
                    .Expect<BalloonTitleChanged>(e => e.SourceId == syncCommand.AggregateId)
                    .Execute(TimeSpan.FromMilliseconds(100));

            await Assert.ThrowsAsync<TimeoutException>(() => waiter);
        }
    }
}