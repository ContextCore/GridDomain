using System;
using System.Threading.Tasks;

namespace GridDomain.Tests.XUnit.CommandsExecution
{
  
    public class When_execute_command_causing_an_aggregate_error : SampleDomainCommandExecutionTests
    {
       [Fact]
        public async Task Given_sync_aggregate_method_Then_execute_throws_exception_from_aggregate()
        {
            var syncCommand = new AlwaysFaultCommand(Guid.NewGuid());

            await Node.Prepare(syncCommand)
                          .Execute()
                          .ShouldThrow<SampleAggregateException>();
        }


       [Fact]
        public async Task Given_sync_aggregate_method_Then_execute_throws_exception_from_aggregate_with_stack_trace()
        {
            var syncCommand = new AlwaysFaultCommand(Guid.NewGuid());

            await Node.Prepare(syncCommand)
                          .Execute()
                          .ShouldThrow<SampleAggregateException>(ex => ex.StackTrace.Contains(typeof(SampleAggregate).Name));
        }

       [Fact]
        public async Task Given_async_aggregate_method_Then_execute_throws_exception_from_aggregate()
        {
            var syncCommand = new AlwaysFaultAsyncCommand(Guid.NewGuid());

            await Node.Prepare(syncCommand)
                          .Execute()
                          .ShouldThrow<SampleAggregateException>();
        }


       [Fact]
        public async Task Given_async_aggregate_method_Then_execute_throws_exception_from_aggregate_with_stack_trace()
        {
            var syncCommand = new AsyncFaultWithOneEventCommand(42, Guid.NewGuid(), Guid.NewGuid(), TimeSpan.FromMilliseconds(500));

            await Node.Prepare(syncCommand)
                          .Execute()
                          .ShouldThrow<SampleAggregateException>(ex => ex.StackTrace.Contains(typeof(SampleAggregate).Name));
        }
    }
}