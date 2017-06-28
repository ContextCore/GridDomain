using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging;
using GridDomain.Tests.Unit.BalloonDomain.Events;

namespace GridDomain.Tests.Unit.BalloonDomain.ProjectionBuilders
{
    public class BalloonTitleChangedFaultyMessageHandler : IHandler<BalloonTitleChanged>
    {
        private readonly IPublisher _publisher;

        public BalloonTitleChangedFaultyMessageHandler(IPublisher publisher)
        {
            _publisher = publisher;
        }

        public Task Handle(BalloonTitleChanged msg)
        {
            var i = int.Parse(msg.Value);
            if (i % 2 == 1)
                return Task.Run(() => throw new MessageHandleException(msg));

            _publisher.Publish(new BalloonTitleChangedNotification {BallonId = msg.SourceId});
            return Task.CompletedTask;
        }
    }
}