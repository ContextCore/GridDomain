using GridDomain.CQRS.Messaging;
using GridDomain.CQRS.Messaging.MessageRouting;
using GridDomain.Tests.EventsUpgrade.Domain.Events;
using GridDomain.Tests.EventsUpgrade.Domain.ProjectionBuilders;

namespace GridDomain.Tests.EventsUpgrade.Domain
{
    public class BalanceRouteMap : IMessageRouteMap
    {
        public BalanceRouteMap()
        {
        }

        public void Register(IMessagesRouter router)
        {
            router.RegisterAggregate(BalanceAggregatesCommandHandler.Descriptor);
            router.RegisterHandler<BalanceChangedEvent_V0, SampleProjectionBuilder>(m => m.SourceId);
        }
    }
}