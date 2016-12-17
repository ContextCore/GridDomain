using System;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging;
using GridDomain.Tests.SampleDomain.Events;

namespace GridDomain.Tests.SampleDomain.ProjectionBuilders
{
    public class SampleProjectionBuilder :// IHandler<SampleAggregateChangedEvent>,
                                           IHandlerWithMetadata<SampleAggregateChangedEvent>
    {
        private readonly IPublisher _publisher;

        public SampleProjectionBuilder(IPublisher publisher)
        {
            _publisher = publisher;
        }

        public void Handle(SampleAggregateChangedEvent msg)
        {
            _publisher.Publish(new AggregateChangedEventNotification() { AggregateId = msg.SourceId} );
        }

        public void Handle(SampleAggregateChangedEvent msg, IMessageMetadata metadata)
        {
            var notificationMetadata = metadata.CreateChild(Guid.NewGuid(), 
                                                    new ProcessEntry(nameof(SampleProjectionBuilder),
                                                    "Publishing notification", 
                                                    "Aggregate created event processed"));

            _publisher.Publish(new AggregateChangedEventNotification() { AggregateId = msg.SourceId }, notificationMetadata);
        }
    }
}