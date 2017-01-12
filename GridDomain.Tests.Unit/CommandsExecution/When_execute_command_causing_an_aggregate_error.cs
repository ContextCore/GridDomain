using System;
using System.Threading;
using System.Threading.Tasks;
using GridDomain.Tests.Unit.SampleDomain;
using GridDomain.Tests.Unit.SampleDomain.Commands;
using GridDomain.Tests.Unit.SampleDomain.Events;
using NUnit.Framework;

namespace GridDomain.Tests.Unit.CommandsExecution
{
    [TestFixture]
    public class When_execute_command_causing_an_aggregate_error : InMemorySampleDomainTests
    {
        [Then]
        public async Task Given_sync_aggregate_method_Then_execute_throws_exception_from_aggregate()
        {
            var syncCommand = new AlwaysFaultCommand(Guid.NewGuid());

            await GridNode.PrepareCommand(syncCommand)
                          .Execute()
                          .ShouldThrow<SampleAggregateException>();
        }


        [Then]
        public async Task Given_sync_aggregate_method_Then_execute_throws_exception_from_aggregate_with_stack_trace()
        {
            var syncCommand = new AlwaysFaultCommand(Guid.NewGuid());

            await GridNode.PrepareCommand(syncCommand)
                          .Execute()
                          .ShouldThrow<SampleAggregateException>(ex => ex.StackTrace.Contains(typeof(SampleAggregate).Name));
        }

        [Then]
        public async Task Given_async_aggregate_method_Then_execute_throws_exception_from_aggregate()
        {
            var syncCommand = new AlwaysFaultAsyncCommand(Guid.NewGuid());

            await GridNode.PrepareCommand(syncCommand)
                          .Execute()
                          .ShouldThrow<SampleAggregateException>();
        }


        [Then]
        public async Task Given_async_aggregate_method_Then_execute_throws_exception_from_aggregate_with_stack_trace()
        {
            var syncCommand = new AsyncFaultWithOneEventCommand(42, Guid.NewGuid(), Guid.NewGuid(), TimeSpan.FromMilliseconds(500));

            await GridNode.PrepareCommand(syncCommand)
                          .Execute()
                          .ShouldThrow<SampleAggregateException>(ex => ex.StackTrace.Contains(typeof(SampleAggregate).Name));
        }
    }
}