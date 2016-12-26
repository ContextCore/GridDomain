using GridDomain.CQRS.Messaging.MessageRouting;
using GridDomain.Tests.Unit.SampleDomain.Events;
using GridDomain.Tests.Unit.SampleDomain.ProjectionBuilders;
using Microsoft.Practices.Unity;

namespace GridDomain.Tests.Unit.SampleDomain
{
    class SampleProjectionGroup : ProjectionGroup
    {
        public SampleProjectionGroup(IUnityContainer locator) : base(locator)
        {
            //TODO: only for test purposes!!!
            AggregateChangedProjectionBuilder.ProjectionGroupHashCode = this.GetHashCode();
            AggregateCreatedProjectionBuilder.ProjectionGroupHashCode = this.GetHashCode();
            Add<SampleAggregateChangedEvent,AggregateChangedProjectionBuilder>(nameof(SampleAggregateChangedEvent.SourceId));
            Add<SampleAggregateCreatedEvent,AggregateCreatedProjectionBuilder>(nameof(SampleAggregateCreatedEvent.SourceId));
            Add<SampleAggregateCreatedEvent,AggregateCreatedProjectionBuilder_Alternative>(nameof(SampleAggregateCreatedEvent.SourceId));
        }
    }
}