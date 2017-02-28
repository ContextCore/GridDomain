using System;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.Tests.XUnit.SampleDomain.Commands;
using GridDomain.Tests.XUnit.SampleDomain.Events;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.XUnit.CommandsExecution
{
    public class AsyncExecute_without_timeout : SampleDomainCommandExecutionTests
    {
        public AsyncExecute_without_timeout(ITestOutputHelper output) : base(output) {}

        [Fact]
        public async Task CommandWaiter_throws_exception_after_wait_with_only_default_timeout()
        {
            var syncCommand = new LongOperationCommand(1000, Guid.NewGuid());
            var waiter =
                Node.Prepare(syncCommand)
                    .Expect<SampleAggregateChangedEvent>(e => e.SourceId == syncCommand.AggregateId)
                    .Execute(TimeSpan.FromMilliseconds(100));

            await Assert.ThrowsAsync<TimeoutException>(() => waiter);
        }
    }
}