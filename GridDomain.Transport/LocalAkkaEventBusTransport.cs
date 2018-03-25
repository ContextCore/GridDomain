using System;
using System.Collections.Generic;
using Akka.Actor;
using Akka.Event;
using GridDomain.Common;

namespace GridDomain.Transport
{
    public class LocalAkkaEventBusTransport : IActorTransport
    {
        private readonly EventStream _bus;
        public readonly IDictionary<Type, List<IActorRef>> Subscribers = new Dictionary<Type, List<IActorRef>>();

        public LocalAkkaEventBusTransport(ActorSystem system)
        {
            _bus = system.EventStream;
        }

        public void Subscribe<TMessage>(IActorRef actor)
        {
            Subscribe(typeof(TMessage), actor);
        }

        public void Unsubscribe(IActorRef actor, Type topic)
        {
            _bus.Unsubscribe(actor, topic);
        }

        public void Subscribe(Type messageType, IActorRef actor, IActorRef subscribeNotificationWaiter)
        {
            Subscribe(messageType, actor);
        }

        public void Publish(object msg)
        {
            _bus.Publish(msg);
        }

        public void Publish(object msg, IMessageMetadata metadata)
        {
            _bus.Publish(new MessageMetadataEnvelop(msg, metadata));
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