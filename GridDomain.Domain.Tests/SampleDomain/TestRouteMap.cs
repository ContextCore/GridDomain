using GridDomain.CQRS.Messaging;
using GridDomain.CQRS.Messaging.MessageRouting;
using Microsoft.Practices.Unity;

namespace GridDomain.Tests.SampleDomain
{
    class TestRouteMap : IMessageRouteMap
    {
        private readonly IUnityContainer _locator;

        public TestRouteMap(IUnityContainer locator)
        {
            _locator = locator;
        }

        public void Register(IMessagesRouter router)
        {
            router.RegisterAggregate(TestAggregatesCommandHandler.Descriptor);
            router.RegisterProjectionGroup(new TestProjectionGroup(_locator));
            router.RegisterHandler<AggregateChangedEvent, SampleProjectionBuilder>(m => m.SourceId);
        }
    }
}