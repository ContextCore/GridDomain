using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging;
using GridDomain.Tests.XUnit.SampleDomain.Events;

namespace GridDomain.Tests.XUnit.SampleDomain.ProjectionBuilders
{
    public class EvenFaultyMessageHandler : IHandler<SampleAggregateChangedEvent>
    {
        private readonly IPublisher _publisher;

        public EvenFaultyMessageHandler(IPublisher publisher)
        {
            _publisher = publisher;
        }

        public Task Handle(SampleAggregateChangedEvent msg)
        {
            var i = int.Parse(msg.Value);
            if (i%2 == 1)
                return Task.Run(() => { throw new MessageHandleException(msg); });

            _publisher.Publish(new AggregateChangedEventNotification {AggregateId = msg.SourceId});
            return Task.CompletedTask;
        }
    }
}