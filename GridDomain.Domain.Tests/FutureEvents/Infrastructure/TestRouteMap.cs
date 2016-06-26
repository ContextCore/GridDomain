using GridDomain.CQRS.Messaging;
using GridDomain.CQRS.Messaging.MessageRouting;

namespace GridDomain.Tests.FutureEvents.Infrastructure
{
    class TestRouteMap : IMessageRouteMap
    {
        public void Register(IMessagesRouter router)
        {
            router.RegisterAggregate(TestAggregatesCommandHandler.Descriptor);
        }
    }
}