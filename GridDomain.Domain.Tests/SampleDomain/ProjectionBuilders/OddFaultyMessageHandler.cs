using System;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging;
using GridDomain.Tests.SampleDomain.Events;

namespace GridDomain.Tests.SampleDomain.ProjectionBuilders
{
    public class OddFaultyMessageHandler : IHandler<SampleAggregateChangedEvent>
    {
        private readonly IPublisher _publisher;

        public OddFaultyMessageHandler(IPublisher publisher)
        {
            _publisher = publisher;
        }

        public void Handle(SampleAggregateChangedEvent msg)
        {
            var i = int.Parse(msg.Value);
            if(i % 2 == 0)
                throw new MessageHandleException(msg);

            _publisher.Publish(new AggregateChangedEventNotification() { AggregateId = msg.SourceId });
        }

    }

    public class MessageHandleException : Exception
    {
        public readonly SampleAggregateChangedEvent Msg;

        public MessageHandleException(SampleAggregateChangedEvent msg)
        {
            Msg = msg;
        }
    }

    public class EvenFaultyMessageHandler : IHandler<SampleAggregateChangedEvent>
    {
        private readonly IPublisher _publisher;

        public EvenFaultyMessageHandler(IPublisher publisher)
        {
            _publisher = publisher;
        }

        public void Handle(SampleAggregateChangedEvent msg)
        {
            var i = int.Parse(msg.Value);
            if (i % 2 == 1)
                throw new MessageHandleException(msg);

            _publisher.Publish(new AggregateChangedEventNotification() { AggregateId = msg.SourceId });
        }
    }
}