using System;
using System.Collections.Generic;
using System.Linq;
using Akka.Actor;
using Akka.Cluster.Tools.PublishSubscribe;
using Akka.Event;
using GridDomain.Logging;

namespace GridDomain.CQRS.Messaging.Akka
{
    public class LocalAkkaEventBusTransport : IActorTransport
    {
        private readonly EventStream _bus;
        public readonly IDictionary<Type,List<IActorRef>> Subscribers = new Dictionary<Type, List<IActorRef>>();
        private readonly ISoloLogger _log = LogManager.GetLogger();

        public LocalAkkaEventBusTransport(ActorSystem system, LogLevel? level = null)
        {
            _bus = system.EventStream;
            _bus.SetLogLevel(LogLevel.DebugLevel);
          //  if(level.HasValue)
          //      _bus.SetLogLevel(level.Value);
        }

        public void Subscribe<TMessage>(IActorRef actor)
        {
            Subscribe(typeof (TMessage), actor);
        }

        public void Unsubscribe(IActorRef actor, Type topic)
        {
            _bus.Unsubscribe(actor,topic);
        }

        public void Subscribe(Type messageType, IActorRef actor, IActorRef subscribeNotificationWaiter)
        {
            Subscribe(messageType, actor);
        }

        public void Publish(params object[] messages)
        {
            foreach (var msg in messages)
            {
                var subscribers = 
                    Subscribers.Where(s => s.Key.IsInstanceOfType(msg))
                               .SelectMany(s => s.Value)
                               .Select(p => p.Path.ToString());

                _log.Trace("Publishing {@Message} to transport, possible receivers {@receivers}",
                    msg, String.Join(";",subscribers));

                _bus.Publish(msg);
            }
        }

        public void Subscribe(Type messageType, IActorRef actor)
        {
            _bus.Subscribe(actor, messageType);

            List<IActorRef> subscribers;
            if (!Subscribers.TryGetValue(messageType, out subscribers))
            {
                subscribers = new List<IActorRef>();
                Subscribers[messageType] = subscribers;
            }
            subscribers.Add(actor);

        }
    }
}