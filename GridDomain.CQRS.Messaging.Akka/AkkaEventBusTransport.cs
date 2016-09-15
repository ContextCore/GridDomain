using System;
using System.Collections.Generic;
using Akka.Actor;
using Akka.Event;
using GridDomain.Logging;

namespace GridDomain.CQRS.Messaging.Akka
{
    public class AkkaEventBusTransport : IActorTransport
    {
        private readonly EventStream _bus;
        public readonly IDictionary<Type,List<IActorRef>> Subscribers = new Dictionary<Type, List<IActorRef>>();
        private ISoloLogger _log = LogManager.GetLogger();

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
            _log.Trace("Publishing {Message} to transport", msg);
            _bus.Publish(msg);
        }

        public void Subscribe(Type messageType, IActorRef actor)
        {
            List<IActorRef> subscribers;
            if (!Subscribers.TryGetValue(messageType, out subscribers))
            {
                subscribers = new List<IActorRef>();
                Subscribers[messageType] = subscribers;
            }
            subscribers.Add(actor);

            _bus.Subscribe(actor, messageType);
        }
    }
}