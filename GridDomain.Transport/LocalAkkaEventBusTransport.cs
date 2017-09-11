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
        private readonly ILoggingAdapter _log;
        public readonly IDictionary<Type, List<IActorRef>> Subscribers = new Dictionary<Type, List<IActorRef>>();

        public LocalAkkaEventBusTransport(ActorSystem system)
        {
            _bus = system.EventStream;
            _log = system.Log;
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
            // _log.Debug("Publishing {Message} to transport", msg);
            Publish(msg, MessageMetadata.Empty);
        }

        public void Publish(object msg, IMessageMetadata metadata)
        {
            //   _log.Debug("Publishing {Message} to transport with metadata", msg);
            var messageMetadataEnvelop = MessageMetadataEnvelop.NewTyped(msg, metadata);
            _bus.Publish(messageMetadataEnvelop);
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