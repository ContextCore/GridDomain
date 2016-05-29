using System;
using Akka.Actor;
using Akka.Event;
using GridDomain.CQRS.Messaging;

namespace GridDomain.Node.AkkaMessaging
{
    public class AkkaEventBusTransport : IActorSubscriber, IPublisher
    {
        private EventStream _bus;

        public AkkaEventBusTransport(ActorSystem system)
        {
            _bus = system.EventStream;
        }

        public void Subscribe<TMessage>(IActorRef actor)
        {
            Subscribe(typeof (TMessage), actor);
        }

        public void Subscribe(Type messageType, IActorRef actor)
        {
            _bus.Subscribe(actor, messageType);

        }

        public void Publish<T>(T msg)
        {
            _bus.Publish(msg);
        }
    }
}