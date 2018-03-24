using System;
using GridDomain.CQRS;
using GridDomain.Tests.Unit.BalloonDomain.Commands;
using GridDomain.Tests.Unit.BalloonDomain.Events;
using GridDomain.Tests.Unit.CommandsExecution.ExecutionWithErrors;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.CommandsExecution
{
    public class SyncExecute_with_timeout : BalloonDomainCommandExecutionTests
    {
        public SyncExecute_with_timeout(ITestOutputHelper output) : base(output) {}

        [Fact]
        public void CommandWaiter_doesnt_throw_exception_after_wait_with_timeout()
        {
            var syncCommand = new PlanTitleWriteCommand(1000, Guid.NewGuid());
            Node.Prepare(syncCommand)
                .Expect<BalloonTitleChanged>(e => e.SourceId == syncCommand.AggregateId)
                .Execute(TimeSpan.FromMilliseconds(500))
                .Wait(100);
        }
    }
}