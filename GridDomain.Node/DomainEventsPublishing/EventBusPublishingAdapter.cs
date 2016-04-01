using Akka.Event;
using GridDomain.CQRS.Messaging;

namespace GridDomain.Node.DomainEventsPublishing
{
    internal class EventBusPublishingAdapter : IPublisher
    {
        private readonly EventStream _eventStream;

        public EventBusPublishingAdapter(EventStream eventStream)
        {
            _eventStream = eventStream;
        }

        public void Publish<T>(T msg)
        {
            _eventStream.Publish(msg);
        }
    }
}