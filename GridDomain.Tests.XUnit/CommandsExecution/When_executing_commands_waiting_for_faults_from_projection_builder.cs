using GridDomain.CQRS.Messaging;
using GridDomain.CQRS.Messaging.MessageRouting;

namespace GridDomain.Tests.XUnit.CommandsExecution
{
  

    public class When_executing_commands_waiting_for_faults_from_projection_builder : CommandWaiter_waits_for_faults<MessageHandleException>
    {
        protected override IMessageRouteMap CreateMap()
        {
            return new CustomRouteMap(
                r => r.RegisterHandler<SampleAggregateChangedEvent, OddFaultyMessageHandler>(e => e.SourceId),
                r => r.RegisterHandler<SampleAggregateCreatedEvent, FaultyCreateProjectionBuilder>(e => e.SourceId),
                r => r.RegisterAggregate(SampleAggregatesCommandHandler.Descriptor));
        }
    }
}