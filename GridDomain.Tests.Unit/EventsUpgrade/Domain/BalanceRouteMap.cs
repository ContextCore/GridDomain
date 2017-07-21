using System.Threading.Tasks;
using GridDomain.Routing;
using GridDomain.Routing.MessageRouting;
using GridDomain.Tests.Unit.EventsUpgrade.Domain.Events;
using GridDomain.Tests.Unit.EventsUpgrade.Domain.ProjectionBuilders;

namespace GridDomain.Tests.Unit.EventsUpgrade.Domain
{
    public class BalanceRouteMap : IMessageRouteMap
    {
        public async Task Register(IMessagesRouter router)
        {
            await router.RegisterAggregate<BalanceAggregate,BalanceAggregatesCommandHandler>();
            await router.RegisterSyncHandler<BalanceChangedEvent_V0, SampleProjectionBuilder>();
        }

        public string Name { get; } = nameof(BalanceRouteMap);
    }
}