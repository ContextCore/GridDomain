using System.Threading.Tasks;
using GridDomain.CQRS.Messaging;
using GridDomain.CQRS.Messaging.MessageRouting;
using GridDomain.Tests.Unit.BalloonDomain;
using GridDomain.Tests.Unit.BalloonDomain.Events;

namespace GridDomain.Tests.Acceptance.BalloonDomain
{
    public class BalloonWithProjectionRouteMap : IMessageRouteMap
    {
        public async Task Register(IMessagesRouter router)
        {
            await router.RegisterAggregate(BalloonCommandHandler.Descriptor);
            await router.RegisterHandler<BalloonTitleChanged, BalloonCatalogProjection>(m => m.SourceId);
            await router.RegisterHandler<BalloonCreated, BalloonCatalogProjection>(m => m.SourceId);
        }
    }
}