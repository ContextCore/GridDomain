using System.Threading.Tasks;
using GridDomain.CQRS.Messaging;
using GridDomain.CQRS.Messaging.MessageRouting;
using GridDomain.EventSourcing;
using GridDomain.Tests.SampleDomain.Events;
using GridDomain.Tests.SampleDomain.ProjectionBuilders;
using Microsoft.Practices.Unity;

namespace GridDomain.Tests.SampleDomain
{
    public class SampleRouteMap : IMessageRouteMap
    {
        private readonly IUnityContainer _locator;

        public SampleRouteMap(IUnityContainer locator)
        {
            _locator = locator;
        }

        public async Task Register(IMessagesRouter router)
        {
            await router.RegisterAggregate(SampleAggregatesCommandHandler.Descriptor);
            await router.RegisterProjectionGroup(new SampleProjectionGroup(_locator));
            await router.RegisterHandler<SampleAggregateChangedEvent, SampleProjectionBuilder>(m => m.SourceId);

        }
    }
}