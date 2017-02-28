using System;
using System.Threading.Tasks;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging;
using GridDomain.Tests.XUnit.SampleDomain.Events;

namespace GridDomain.Tests.XUnit.SampleDomain.ProjectionBuilders
{
    public class SampleProjectionBuilder : IHandlerWithMetadata<SampleAggregateChangedEvent>
    {
        private readonly IPublisher _publisher;

        public SampleProjectionBuilder(IPublisher publisher)
        {
            _publisher = publisher;
        }

        public Task Handle(SampleAggregateChangedEvent msg)
        {
            _publisher.Publish(new AggregateChangedEventNotification {AggregateId = msg.SourceId});
            return Task.CompletedTask;
        }

        public Task Handle(SampleAggregateChangedEvent msg, IMessageMetadata metadata)
        {
            var notificationMetadata = metadata.CreateChild(Guid.NewGuid(),
                new ProcessEntry(nameof(SampleProjectionBuilder),
                    "Publishing notification",
                    "Aggregate created event processed"));

            _publisher.Publish(new AggregateChangedEventNotification {AggregateId = msg.SourceId}, notificationMetadata);
            return Task.CompletedTask;
        }
    }
}