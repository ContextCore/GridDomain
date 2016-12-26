using System;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.Tests.Unit.SampleDomain;
using GridDomain.Tests.Unit.SampleDomain.Commands;
using GridDomain.Tests.Unit.SampleDomain.ProjectionBuilders;
using NUnit.Framework;

namespace GridDomain.Tests.Unit.CommandsExecution.ExpectedMessages
{
    [TestFixture]
    public class When_wait_execution__of_async_method_until_projection_build_event_wait_by_Node : SampleDomainCommandExecutionTests
    {
        public When_wait_execution__of_async_method_until_projection_build_event_wait_by_Node():base(true)
        {
            
        }
        [Then]
        public async Task After_wait_aggregate_should_be_changed()
        {
            var syncCommand = new AsyncMethodCommand(42, Guid.NewGuid());
            var expectedMessage = Expect.Message<AggregateChangedEventNotification>(e => e.AggregateId,
                syncCommand.AggregateId);

            await GridNode.Execute(CommandPlan.New(syncCommand, Timeout, expectedMessage));

            var aggregate = LoadAggregate<SampleAggregate>(syncCommand.AggregateId);
            Assert.AreEqual(syncCommand.Parameter.ToString(), aggregate.Value);
        }
    }
}