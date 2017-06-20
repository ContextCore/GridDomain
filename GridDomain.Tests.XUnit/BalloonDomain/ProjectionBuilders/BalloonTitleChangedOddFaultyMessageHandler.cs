using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging;
using GridDomain.Tests.XUnit.BalloonDomain.Events;

namespace GridDomain.Tests.XUnit.BalloonDomain.ProjectionBuilders
{
    public class BalloonTitleChangedOddFaultyMessageHandler : IHandler<BalloonTitleChanged>
    {
        private readonly IPublisher _publisher;

        public BalloonTitleChangedOddFaultyMessageHandler(IPublisher publisher)
        {
            _publisher = publisher;
        }

        public Task Handle(BalloonTitleChanged msg)
        {
            var i = int.Parse(msg.Value);

            if (i % 8 == 0)
                return Task.Run(() => { throw new MessageHandleException(msg); });

            if (i % 2 == 0)
                throw new MessageHandleException(msg);

            _publisher.Publish(new AggregateChangedEventNotification {AggregateId = msg.SourceId});

            return Task.CompletedTask;
        }
    }
}