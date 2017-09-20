using System;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.TestKit.Xunit2;
using GridDomain.Tests.Common;
using GridDomain.Tests.Unit.BalloonDomain;
using GridDomain.Tests.Unit.BalloonDomain.Commands;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.CommandsExecution
{
    public class When_execute_command_causing_an_aggregate_error : BalloonDomainCommandExecutionTests
    {
        public When_execute_command_causing_an_aggregate_error(ITestOutputHelper output) : base(output) {}

        [Fact]
        public async Task Given_async_aggregate_method_Then_execute_throws_exception_from_aggregate()
        {
            await Node.Execute(new PlanBallonBlowCommand(Guid.NewGuid()))
                      .CommandShouldThrow<BalloonException>();
        }

        [Fact]
        public async Task Given_async_aggregate_method_Then_execute_throws_exception_from_aggregate_with_stack_trace()
        {
            var syncCommand = new PlanTitleWriteAndBlowCommand(42,
                                                                Guid.NewGuid(),
                                                                TimeSpan.FromMilliseconds(500));

            await Node.Execute(syncCommand)
                      .CommandShouldThrow<BalloonException>(ex =>
                                                            {
                                                                if (ex.StackTrace == null) return true; //weird but it is due to Hyperion serializer
                                                                return ex.StackTrace.Contains(typeof(Balloon).Name);
                                                            });
        }

        [Fact]
        public async Task Given_sync_aggregate_method_Then_execute_throws_exception_from_aggregate_with_stack_trace()
        {
            await Node.Execute(new BlowBalloonCommand(Guid.NewGuid()))
                      .CommandShouldThrow<BalloonException>(ex =>
                                                            {
                                                                if (ex.StackTrace == null) return true; //weird but it is due to Hyperion serializer
                                                                return ex.StackTrace.Contains(typeof(Balloon).Name);
                                                            });
        }
    }
}