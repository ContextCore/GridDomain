using System;
using System.Threading.Tasks;
using GridDomain.Tests.Common;
using GridDomain.Tests.Unit.BalloonDomain;
using GridDomain.Tests.Unit.BalloonDomain.Commands;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.CommandsExecution
{
    public class When_execute_command_causing_an_aggregate_error : SampleDomainCommandExecutionTests
    {
        public When_execute_command_causing_an_aggregate_error(ITestOutputHelper output) : base(output) {}

        [Fact]
        public async Task Given_async_aggregate_method_Then_execute_throws_exception_from_aggregate()
        {
            await Node.Prepare(new PlanBallonBlowCommand(Guid.NewGuid()))
                      .Execute()
                      .ShouldThrow<BalloonException>();
        }

        [Fact]
        public async Task Given_async_aggregate_method_Then_execute_throws_exception_from_aggregate_with_stack_trace()
        {
            var syncCommand = new PlanTitleWriteAndBlowCommand(42,
                                                                Guid.NewGuid(),
                                                                TimeSpan.FromMilliseconds(500));

            await Node.Prepare(syncCommand)
                      .Execute()
                      .ShouldThrow<BalloonException>(ex => ex.StackTrace.Contains(typeof(Balloon).Name));
        }

        [Fact]
        public async Task Given_sync_aggregate_method_Then_execute_throws_exception_from_aggregate_with_stack_trace()
        {
            await Node.Prepare(new BlowBalloonCommand(Guid.NewGuid()))
                      .Execute()
                      .ShouldThrow<BalloonException>(ex => ex.StackTrace.Contains(typeof(Balloon).Name));
        }
    }
}