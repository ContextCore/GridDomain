using GridDomain.CQRS.Messaging;
using GridDomain.CQRS.Messaging.MessageRouting;

namespace GridDomain.Tests.SyncProjection.SampleDomain
{
    class TestRouteMap : IMessageRouteMap
    {
        public void Register(IMessagesRouter router)
        {
            router.RegisterAggregate(TestAggregatesCommandHandler.Descriptor);
            router.RegisterProjectionGroup(new TestProjectionGroup());
            router.Route<AggregateChangedEvent>().ToHandler<SampleProjectionBuilder>();
        }
    }
}