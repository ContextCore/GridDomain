using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging;
using GridDomain.Tests.XUnit.SampleDomain.Events;

namespace GridDomain.Tests.XUnit.SampleDomain.ProjectionBuilders
{
    public class OddFaultyMessageHandler : IHandler<SampleAggregateChangedEvent>
    {
        private readonly IPublisher _publisher;

        public OddFaultyMessageHandler(IPublisher publisher)
        {
            _publisher = publisher;
        }

        public Task Handle(SampleAggregateChangedEvent msg)
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