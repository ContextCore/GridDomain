using GridDomain.CQRS.Messaging.MessageRouting;
using GridDomain.Tests.Acceptance.EventsUpgrade.SampleDomain.Events;
using GridDomain.Tests.Acceptance.EventsUpgrade.SampleDomain.ProjectionBuilders;
using Microsoft.Practices.Unity;

namespace GridDomain.Tests.Acceptance.EventsUpgrade.SampleDomain
{
    class SampleProjectionGroup : ProjectionGroup
    {
        public SampleProjectionGroup(IUnityContainer locator) : base(locator)
        {
            //TODO: only for test purposes!!!
            AggregateChangedProjectionBuilder.ProjectionGroupHashCode = this.GetHashCode();
            AggregateCreatedProjectionBuilder.ProjectionGroupHashCode = this.GetHashCode();
            Add<AggregateCreatedEvent,AggregateCreatedProjectionBuilder>(nameof(AggregateCreatedEvent.SourceId));
        }
    }
}