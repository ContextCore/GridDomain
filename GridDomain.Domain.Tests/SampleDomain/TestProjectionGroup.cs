using GridDomain.CQRS.Messaging;
using GridDomain.CQRS.Messaging.MessageRouting;

namespace GridDomain.Tests.SampleDomain
{
    class TestProjectionGroup : ProjectionGroup
    {
        public TestProjectionGroup(IServiceLocator locator) : base(locator)
        {
            //TODO: only for test purposes!!!
            AggregateChangedProjectionBuilder.ProjectionGroupHashCode = this.GetHashCode();
            AggregateCreatedProjectionBuilder.ProjectionGroupHashCode = this.GetHashCode();
            Add<AggregateChangedEvent,AggregateChangedProjectionBuilder>(nameof(AggregateChangedEvent.SourceId));
            Add<AggregateCreatedEvent,AggregateCreatedProjectionBuilder>(nameof(AggregateCreatedEvent.SourceId));
            Add<AggregateCreatedEvent,AggregateCreatedProjectionBuilder_Alternative>(nameof(AggregateCreatedEvent.SourceId));
        }
    }
}