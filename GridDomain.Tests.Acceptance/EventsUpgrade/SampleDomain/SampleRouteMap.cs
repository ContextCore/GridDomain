using GridDomain.CQRS.Messaging;
using GridDomain.CQRS.Messaging.MessageRouting;
using GridDomain.Tests.Acceptance.EventsUpgrade.SampleDomain.Events;
using GridDomain.Tests.Acceptance.EventsUpgrade.SampleDomain.ProjectionBuilders;
using Microsoft.Practices.Unity;

namespace GridDomain.Tests.Acceptance.EventsUpgrade.SampleDomain
{
    class SampleRouteMap : IMessageRouteMap
    {
        private readonly IUnityContainer _locator;

        public SampleRouteMap(IUnityContainer locator)
        {
            _locator = locator;
        }

        public void Register(IMessagesRouter router)
        {
            router.RegisterAggregate(SampleAggregatesCommandHandler.Descriptor);
            router.RegisterProjectionGroup(new SampleProjectionGroup(_locator));
            router.RegisterHandler<BalanceChangedEvent, SampleProjectionBuilder>(m => m.SourceId);
        }
    }
}