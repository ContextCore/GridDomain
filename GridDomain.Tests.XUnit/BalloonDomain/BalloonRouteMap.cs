using System.Threading.Tasks;
using GridDomain.CQRS.Messaging;
using GridDomain.CQRS.Messaging.MessageRouting;
using GridDomain.Tests.XUnit.BalloonDomain.Events;
using GridDomain.Tests.XUnit.BalloonDomain.ProjectionBuilders;

namespace GridDomain.Tests.XUnit.BalloonDomain
{
    public class BalloonRouteMap : IMessageRouteMap
    {
        public async Task Register(IMessagesRouter router)
        {
            await router.RegisterAggregate(BalloonCommandHandler.Descriptor);
            await router.RegisterHandler<BalloonTitleChanged, SampleProjectionBuilder>(m => m.SourceId);
            await router.RegisterHandler<BalloonTitleChanged, AggregateChangedProjectionBuilder>(m => m.SourceId);
            await router.RegisterHandler<BalloonCreated, AggregateCreatedProjectionBuilder>(m => m.SourceId);
            await router.RegisterHandler<BalloonCreated, AggregateCreatedProjectionBuilder_Alternative>(m => m.SourceId);
        }
    }
}