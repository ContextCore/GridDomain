using System.Threading.Tasks;
using GridDomain.Configuration.MessageRouting;
using GridDomain.Tests.Unit.EventsUpgrade.Domain.Events;
using GridDomain.Tests.Unit.EventsUpgrade.Domain.ProjectionBuilders;

namespace GridDomain.Tests.Unit.EventsUpgrade.Domain
{
    public class BalanceRouteMap : IMessageRouteMap
    {
        public async Task Register(IMessagesRouter router)
        {
            await router.RegisterAggregate(new BalanceAggregateDescriptor());
            await router.RegisterSyncHandler<BalanceChangedEvent_V0, SampleProjectionBuilder>();
            await router.RegisterSyncHandler<BalanceChangedEvent_V1, SampleProjectionBuilder>();
        }
    }
}