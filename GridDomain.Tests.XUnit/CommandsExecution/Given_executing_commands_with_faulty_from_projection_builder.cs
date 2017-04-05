using System;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging;
using GridDomain.CQRS.Messaging.MessageRouting;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Tests.XUnit.BalloonDomain;
using GridDomain.Tests.XUnit.BalloonDomain.Commands;
using GridDomain.Tests.XUnit.BalloonDomain.Events;
using GridDomain.Tests.XUnit.BalloonDomain.ProjectionBuilders;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.XUnit.CommandsExecution
{
    public class Given_executing_commands_with_faulty_from_projection_builder : NodeTestKit
    {
        public Given_executing_commands_with_faulty_from_projection_builder(ITestOutputHelper output)
            : base(output, new NodeTestFixture(new BalloonContainerConfiguration(), CreateMap())) {}

        private static IMessageRouteMap CreateMap()
        {
            return new CustomRouteMap(r => r.RegisterHandler<BalloonTitleChanged, OddFaultyMessageHandler>(e => e.SourceId),
                                      r => r.RegisterHandler<BalloonCreated, FaultyCreateProjectionBuilder>(e => e.SourceId),
                                      r => r.RegisterAggregate(BalloonCommandHandler.Descriptor));
        }

        [Fact]
        public async Task When_dont_expect_fault_Then_it_is_not_received_case_it_comes_from_projection_builder()
        {
            await Node.Prepare(new PlanTitleWriteCommand(100, Guid.NewGuid()))
                      .Expect<AggregateChangedEventNotification>()
                      .Execute(TimeSpan.FromSeconds(1))
                      .ShouldThrow<TimeoutException>();
        }

        [Fact]
        public async Task When_expect_fault_and_ignore_fault_errors_Then_fault_is_received_and_contains_error_B()
        {
            var syncCommand = new PlanTitleWriteCommand(100, Guid.NewGuid());
            var res = await Node.Prepare(syncCommand)
                                .Expect<AggregateChangedEventNotification>()
                                .Or<Fault<BalloonTitleChanged>>(f => f.Message.SourceId == syncCommand.AggregateId)
                                .Execute(false);

            Assert.IsAssignableFrom<MessageHandleException>(res.Message<IFault<BalloonTitleChanged>>()?.Exception);
        }

        [Fact]
        public async Task When_expect_fault_Then_exception_is_raised()
        {
            await Node.Prepare(new PlanTitleWriteCommand(8, Guid.NewGuid()))
                      .Expect<AggregateChangedEventNotification>()
                      .Or<Fault<BalloonTitleChanged>>()
                      .Execute()
                      .ShouldThrow<MessageHandleException>();
        }
    }
}