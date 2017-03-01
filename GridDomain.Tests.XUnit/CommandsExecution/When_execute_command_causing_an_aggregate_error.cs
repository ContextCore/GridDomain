using System;
using System.Threading.Tasks;
using GridDomain.Tests.XUnit.SampleDomain;
using GridDomain.Tests.XUnit.SampleDomain.Commands;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.XUnit.CommandsExecution
{
    public class When_execute_command_causing_an_aggregate_error : SampleDomainCommandExecutionTests
    {
        public When_execute_command_causing_an_aggregate_error(ITestOutputHelper output) : base(output) {}

        [Fact]
        public async Task Given_async_aggregate_method_Then_execute_throws_exception_from_aggregate()
        {
            await Node.Prepare(new AlwaysFaultAsyncCommand(Guid.NewGuid())).Execute().ShouldThrow<SampleAggregateException>();
        }

        [Fact]
        public async Task Given_async_aggregate_method_Then_execute_throws_exception_from_aggregate_with_stack_trace()
        {
            var syncCommand = new AsyncFaultWithOneEventCommand(42,
                                                                Guid.NewGuid(),
                                                                Guid.NewGuid(),
                                                                TimeSpan.FromMilliseconds(500));

            await
                Node.Prepare(syncCommand)
                    .Execute()
                    .ShouldThrow<SampleAggregateException>(ex => ex.StackTrace.Contains(typeof(SampleAggregate).Name));
        }

        [Fact]
        public async Task Given_sync_aggregate_method_Then_execute_throws_exception_from_aggregate()
        {
            await Node.Prepare(new AlwaysFaultCommand(Guid.NewGuid())).Execute().ShouldThrow<SampleAggregateException>();
        }

        [Fact]
        public async Task Given_sync_aggregate_method_Then_execute_throws_exception_from_aggregate_with_stack_trace()
        {
            await
                Node.Prepare(new AlwaysFaultCommand(Guid.NewGuid()))
                    .Execute()
                    .ShouldThrow<SampleAggregateException>(ex => ex.StackTrace.Contains(typeof(SampleAggregate).Name));
        }
    }
}