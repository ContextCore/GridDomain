using GridDomain.CQRS.Messaging;
using GridDomain.CQRS.Messaging.MessageRouting;
using GridDomain.Tests.Acceptance.EventsUpgrade.SampleDomain.Events;
using GridDomain.Tests.Acceptance.EventsUpgrade.SampleDomain.ProjectionBuilders;
using Microsoft.Practices.Unity;

namespace GridDomain.Tests.Acceptance.EventsUpgrade.SampleDomain
{
    class BalanceRouteMap : IMessageRouteMap
    {
        public BalanceRouteMap()
        {
        }

        public void Register(IMessagesRouter router)
        {
            router.RegisterAggregate(BalanceAggregatesCommandHandler.Descriptor);
            router.RegisterHandler<BalanceChangedEvent, SampleProjectionBuilder>(m => m.SourceId);
        }
    }
}