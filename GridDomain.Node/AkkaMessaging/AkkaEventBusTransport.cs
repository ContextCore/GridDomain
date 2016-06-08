using System;
using Akka.Actor;
using Akka.Event;
using GridDomain.CQRS.Messaging;

namespace GridDomain.Node.AkkaMessaging
{
    public class AkkaEventBusTransport : IActorSubscriber, IPublisher
    {
        private readonly EventStream _bus;

        public AkkaEventBusTransport(ActorSystem system)
        {
            _bus = system.EventStream;
        }

        public void Subscribe<TMessage>(IActorRef actor)
        {
            Subscribe(typeof (TMessage), actor);
        }

        public void Subscribe(Type messageType, IActorRef actor, IActorRef subscribeNotificationWaiter)
        {
            Subscribe(messageType, actor);
        }

        public void Publish<T>(T msg)
        {
            _bus.Publish(msg);
        }

        public void Subscribe(Type messageType, IActorRef actor)
        {
            _bus.Subscribe(actor, messageType);
        }
    }
}