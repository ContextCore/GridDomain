using System;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.Tests.SampleDomain.Commands;
using GridDomain.Tests.SampleDomain.Events;
using NUnit.Framework;

namespace GridDomain.Tests.CommandsExecution.ExpectedMessages
{
    [TestFixture]
    public class CommandPlan_Execute_without_timeout : InMemorySampleDomainTests
    {

        [Then]
        public async Task PlanExecute_throw_exception_after_wait_without_timeout()
        {
            var syncCommand = new LongOperationCommand(1000, Guid.NewGuid());
            var expectedMessage = Expect.Message<SampleAggregateChangedEvent>(e => e.SourceId, syncCommand.AggregateId);
            var plan = CommandPlan.New(syncCommand, TimeSpan.FromMilliseconds(100), expectedMessage);

            await GridNode.Execute(plan)
                .ShouldThrow<TimeoutException>();
        }

    }
}