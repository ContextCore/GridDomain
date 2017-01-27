using System.Threading.Tasks;
using GridDomain.CQRS.Messaging;
using GridDomain.CQRS.Messaging.MessageRouting;
using GridDomain.Tests.XUnit.EventsUpgrade.Domain.Events;
using GridDomain.Tests.XUnit.EventsUpgrade.Domain.ProjectionBuilders;

namespace GridDomain.Tests.XUnit.EventsUpgrade.Domain
{
    public class BalanceRouteMap : IMessageRouteMap
    {

        public async Task Register(IMessagesRouter router)
        {
            await router.RegisterAggregate(BalanceAggregatesCommandHandler.Descriptor);
            await router.RegisterHandler<BalanceChangedEvent_V0, SampleProjectionBuilder>(m => m.SourceId);
        }
    }
}