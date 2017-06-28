using System.Threading.Tasks;
using GridDomain.CQRS.Messaging;
using GridDomain.CQRS.Messaging.MessageRouting;
using GridDomain.Tests.Unit.EventsUpgrade.Domain.Events;
using GridDomain.Tests.Unit.EventsUpgrade.Domain.ProjectionBuilders;

namespace GridDomain.Tests.Unit.EventsUpgrade.Domain
{
    public class BalanceRouteMap : IMessageRouteMap
    {
        public async Task Register(IMessagesRouter router)
        {
            await router.RegisterAggregate<BalanceAggregate,BalanceAggregatesCommandHandler>();
            await router.RegisterHandler<BalanceChangedEvent_V0, SampleProjectionBuilder>(m => m.SourceId);
        }

        public string Name { get; } = nameof(BalanceRouteMap);
    }
}