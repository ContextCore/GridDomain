using System;
using Akka.Actor;
using Akka.Event;
using GridDomain.CQRS.Messaging;

namespace GridDomain.Node.DomainEventsPublishing
{
    internal class EventBusSubscriberAdapter : ISubscriber
    {
        private readonly Func<Type, IActorRef> _actorRefFactory;
        private readonly EventStream _eventStream;

        public EventBusSubscriberAdapter(EventStream eventStream, Func<Type, IActorRef> actorRefFactoryFactory)
        {
            _actorRefFactory = actorRefFactoryFactory;
            _eventStream = eventStream;
        }

        public void Subscribe<TMessage>(Action<TMessage> msgHandler)
        {
            _eventStream.Subscribe(_actorRefFactory(typeof (TMessage)), typeof (TMessage));
        }
    }
}