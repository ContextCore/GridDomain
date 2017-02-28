using System;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging;
using GridDomain.CQRS.Messaging.MessageRouting;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Tests.XUnit.SampleDomain;
using GridDomain.Tests.XUnit.SampleDomain.Commands;
using GridDomain.Tests.XUnit.SampleDomain.Events;
using GridDomain.Tests.XUnit.SampleDomain.ProjectionBuilders;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.XUnit.CommandsExecution
{
    public class Given_executing_commands_with_faulty_from_projection_builder : NodeTestKit
    {
        public Given_executing_commands_with_faulty_from_projection_builder(ITestOutputHelper output)
            : base(output, new NodeTestFixture(new SampleDomainContainerConfiguration(), CreateMap())) {}

        private static IMessageRouteMap CreateMap()
        {
            return
                new CustomRouteMap(
                    r => r.RegisterHandler<SampleAggregateChangedEvent, OddFaultyMessageHandler>(e => e.SourceId),
                    r => r.RegisterHandler<SampleAggregateCreatedEvent, FaultyCreateProjectionBuilder>(e => e.SourceId),
                    r => r.RegisterAggregate(SampleAggregatesCommandHandler.Descriptor));
        }

        [Fact]
        public async Task When_dont_expect_fault_Then_it_is_not_received_case_it_comes_from_projection_builder()
        {
            await
                Node.Prepare(new LongOperationCommand(100, Guid.NewGuid()))
                    .Expect<AggregateChangedEventNotification>()
                    .Execute(TimeSpan.FromSeconds(1))
                    .ShouldThrow<TimeoutException>();
        }

        [Fact]
        public async Task When_expect_fault_and_ignore_fault_erorrs_Then_fault_is_received_and_contains_error_B()
        {
            var syncCommand = new LongOperationCommand(100, Guid.NewGuid());
            var res =
                await
                    Node.Prepare(syncCommand)
                        .Expect<AggregateChangedEventNotification>()
                        .Or<IFault<SampleAggregateChangedEvent>>(f => f.Message.SourceId == syncCommand.AggregateId)
                        .Execute(false);

            Assert.IsAssignableFrom<MessageHandleException>(res.Message<IFault<SampleAggregateChangedEvent>>()?.Exception);
        }

        [Fact]
        public async Task When_expect_fault_Then_exception_is_raised()
        {
            await
                Node.Prepare(new LongOperationCommand(8, Guid.NewGuid()))
                    .Expect<AggregateChangedEventNotification>()
                    .Or<IFault<SampleAggregateChangedEvent>>()
                    .Execute()
                    .ShouldThrow<MessageHandleException>();
        }
    }
}