using System;
using System.Threading.Tasks;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging;
using GridDomain.Tests.XUnit.BalloonDomain.Events;

namespace GridDomain.Tests.XUnit.BalloonDomain.ProjectionBuilders
{
    public class BalloonTitleChangedNotificator : IHandlerWithMetadata<BalloonTitleChanged>
    {
        private readonly IPublisher _publisher;

        public BalloonTitleChangedNotificator(IPublisher publisher)
        {
            _publisher = publisher;
        }

        public Task Handle(BalloonTitleChanged msg)
        {
            _publisher.Publish(new AggregateChangedEventNotification {AggregateId = msg.SourceId});
            return Task.CompletedTask;
        }

        public Task Handle(BalloonTitleChanged msg, IMessageMetadata metadata)
        {
            var notificationMetadata = metadata.CreateChild(Guid.NewGuid(),
                                                            new ProcessEntry(nameof(BalloonTitleChangedNotificator),
                                                                             "Publishing notification",
                                                                             "Aggregate created event processed"));

            _publisher.Publish(new AggregateChangedEventNotification {AggregateId = msg.SourceId}, notificationMetadata);
            return Task.CompletedTask;
        }
    }
}