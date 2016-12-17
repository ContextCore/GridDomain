using GridDomain.Common;
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

        public void Register(IMessagesRouter router)
        {
            router.RegisterAggregate(SampleAggregatesCommandHandler.Descriptor);
            router.RegisterProjectionGroup(new SampleProjectionGroup(_locator));
            router.RegisterHandler<SampleAggregateChangedEvent, SampleProjectionBuilder>(nameof(DomainEvent.SourceId));

        }
    }
}