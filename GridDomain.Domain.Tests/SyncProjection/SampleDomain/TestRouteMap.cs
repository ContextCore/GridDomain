using GridDomain.CQRS.Messaging;
using GridDomain.CQRS.Messaging.MessageRouting;

namespace GridDomain.Tests.SyncProjection.SampleDomain
{
    class TestRouteMap : IMessageRouteMap
    {
        private readonly IServiceLocator _locator;

        public TestRouteMap(IServiceLocator locator)
        {
            _locator = locator;
        }

        public void Register(IMessagesRouter router)
        {
            router.RegisterAggregate(TestAggregatesCommandHandler.Descriptor);
            router.RegisterProjectionGrop(new TestProjectionGroup(_locator));
        }
    }
}