using GridDomain.CQRS.Messaging;
using GridDomain.CQRS.Messaging.MessageRouting;
using GridDomain.Tests.Unit.SampleDomain;
using GridDomain.Tests.XUnit.SampleDomain.Events;
using GridDomain.Tests.XUnit.SampleDomain.ProjectionBuilders;
using Xunit.Abstractions;
using SampleAggregatesCommandHandler = GridDomain.Tests.XUnit.SampleDomain.SampleAggregatesCommandHandler;

namespace GridDomain.Tests.XUnit.CommandsExecution
{
  

    public class When_executing_commands_waiting_for_faults_from_projection_builder : CommandWaiter_waits_for_faults<MessageHandleException>
    {
        public When_executing_commands_waiting_for_faults_from_projection_builder(ITestOutputHelper output) : base(output, 
            new NodeTestFixture(new SampleDomainContainerConfiguration(), CreateMap()))
        {
        }

        private static IMessageRouteMap CreateMap()
        {
            return new CustomRouteMap(
                r => r.RegisterHandler<SampleAggregateChangedEvent, OddFaultyMessageHandler>(e => e.SourceId),
                r => r.RegisterHandler<SampleAggregateCreatedEvent, FaultyCreateProjectionBuilder>(e => e.SourceId),
                r => r.RegisterAggregate(SampleAggregatesCommandHandler.Descriptor));
        }
    }
}