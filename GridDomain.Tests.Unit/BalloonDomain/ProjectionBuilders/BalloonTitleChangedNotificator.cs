using System;
using System.Threading.Tasks;
using GridDomain.Common;
using GridDomain.Configuration.MessageRouting;
using GridDomain.CQRS;
using GridDomain.Tests.Unit.BalloonDomain.Events;

namespace GridDomain.Tests.Unit.BalloonDomain.ProjectionBuilders
{
    public class BalloonTitleChangedNotificator : IHandler<BalloonTitleChanged>
    {
        private readonly IPublisher _publisher;

        public BalloonTitleChangedNotificator(IPublisher publisher)
        {
            _publisher = publisher;
        }

        public Task Handle(BalloonTitleChanged msg)
        {
            _publisher.Publish(new BalloonTitleChangedNotification {BallonId = msg.SourceId});
            return Task.CompletedTask;
        }

        public Task Handle(BalloonTitleChanged msg, IMessageMetadata metadata)
        {
            var notificationMetadata = MessageMetadataExtensions.CreateChild(metadata,
                                                            Guid.NewGuid().ToString(),
                                                            new ProcessEntry(nameof(BalloonTitleChangedNotificator),
                                                                             "Publishing notification",
                                                                             "Aggregate created event processed"));

            _publisher.Publish(new BalloonTitleChangedNotification {BallonId = msg.SourceId}, notificationMetadata);
            return Task.CompletedTask;
        }
    }
}