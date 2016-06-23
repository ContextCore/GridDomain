using GridDomain.CQRS.Messaging;
using GridDomain.CQRS.Messaging.MessageRouting;

namespace GridDomain.Tests.DependencyInjection
{
    class TestRouteMap : IMessageRouteMap
    {
        public void Register(IMessagesRouter router)
        {
            router.RegisterAggregate<TestAggregate, TestAggregatesCommandHandler>();
        }
    }
}