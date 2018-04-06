using System;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Tests.Common;
using GridDomain.Tests.Unit.BalloonDomain.Commands;
using GridDomain.Tests.Unit.BalloonDomain.Events;
using GridDomain.Tests.Unit.BalloonDomain.ProjectionBuilders;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.CommandsExecution.ExecutionWithErrors
{
    public class Given_executing_commands_with_faulty_from_projection_builder : NodeTestKit
    {
        protected Given_executing_commands_with_faulty_from_projection_builder(NodeTestFixture fixture) :base(fixture){}
            
        public Given_executing_commands_with_faulty_from_projection_builder(ITestOutputHelper output)
            : base(new NodeTestFixture(output,new FaultyBalloonProjectionDomainConfiguration())) {}

        [Fact]
        public async Task When_dont_expect_fault_Then_it_is_not_received_case_it_comes_from_projection_builder()
        {
            await Node.Prepare(new PlanTitleWriteCommand(100, Guid.NewGuid()))
                      .Expect<BalloonTitleChangedNotification>()
                      .Execute(TimeSpan.FromSeconds(1))
                      .ShouldThrow<TimeoutException>();
        }

        [Fact]
        public async Task When_expect_fault_and_ignore_fault_errors_Then_fault_is_received_and_contains_error_B()
        {
            var syncCommand = new PlanTitleWriteCommand(100, Guid.NewGuid());
            var res = await Node.Prepare(syncCommand)
                                .Expect<BalloonTitleChangedNotification>()
                                .Or<Fault<BalloonTitleChanged>>(f => f.Message.SourceId == syncCommand.AggregateId)
                                .Execute(false);

            Assert.IsAssignableFrom<MessageHandleException>(res.Message<IFault<BalloonTitleChanged>>()?.Exception);
        }

        [Fact]
        public async Task When_expect_fault_Then_exception_is_raised()
        {
            await Node.Prepare(new PlanTitleWriteCommand(8, Guid.NewGuid()))
                      .Expect<BalloonTitleChangedNotification>()
                      .Or<Fault<BalloonTitleChanged>>()
                      .Execute()
                      .ShouldThrow<MessageHandleException>();
        }
    }
}