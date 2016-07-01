using GridDomain.CQRS.Messaging;
using GridDomain.CQRS.Messaging.MessageRouting;
using GridDomain.Node.Actors;

namespace GridDomain.Tests.SyncProjection.SampleDomain
{
    class TestProjectionGroup : ProjectionGroup
    {
        public TestProjectionGroup(IServiceLocator locator) : base(locator)
        {
            AggregateChangedProjectionBuilder.ProjectionGroupHashCode = this.GetHashCode();
            AggregateCreatedProjectionBuilder.ProjectionGroupHashCode = this.GetHashCode();
            Add<AggregateChangedEvent,AggregateChangedProjectionBuilder>(nameof(AggregateChangedEvent.SourceId));
            Add<AggregateCreatedEvent,AggregateCreatedProjectionBuilder>(nameof(AggregateCreatedEvent.SourceId));
        }
    }
}