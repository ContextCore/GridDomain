using System;
using GridDomain.CQRS;
using GridDomain.Tests.XUnit.SampleDomain.Commands;
using GridDomain.Tests.XUnit.SampleDomain.Events;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.XUnit.CommandsExecution
{
    public class SyncExecute_with_timeout : SampleDomainCommandExecutionTests
    {
        public SyncExecute_with_timeout(ITestOutputHelper output) : base(output) {}

        [Fact]
        public void CommandWaiter_doesnt_throw_exception_after_wait_with_timeout()
        {
            var syncCommand = new LongOperationCommand(1000, Guid.NewGuid());
            Node.Prepare(syncCommand)
                .Expect<SampleAggregateChangedEvent>(e => e.SourceId == syncCommand.AggregateId)
                .Execute(TimeSpan.FromMilliseconds(500))
                .Wait(100);
        }
    }
}