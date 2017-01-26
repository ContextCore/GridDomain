using System.Threading.Tasks;
using GridDomain.CQRS.Messaging;
using GridDomain.CQRS.Messaging.MessageRouting;
using GridDomain.Tests.XUnit.SampleDomain.Events;
using GridDomain.Tests.XUnit.SampleDomain.ProjectionBuilders;

namespace GridDomain.Tests.XUnit.SampleDomain
{
    public class SampleRouteMap : IMessageRouteMap
    {
        public async Task Register(IMessagesRouter router)
        {
            await router.RegisterAggregate(SampleAggregatesCommandHandler.Descriptor);
            await router.RegisterHandler<SampleAggregateChangedEvent, SampleProjectionBuilder>(m => m.SourceId);
            await router.RegisterHandler<SampleAggregateChangedEvent, AggregateChangedProjectionBuilder>(m => m.SourceId);
            await router.RegisterHandler<SampleAggregateCreatedEvent, AggregateCreatedProjectionBuilder>(m => m.SourceId);
            await router.RegisterHandler<SampleAggregateCreatedEvent, AggregateCreatedProjectionBuilder_Alternative>(m => m.SourceId);

        }
    }
}