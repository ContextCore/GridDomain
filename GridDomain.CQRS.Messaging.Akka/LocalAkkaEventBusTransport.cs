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

        public LocalAkkaEventBusTransport(ActorSystem system)
        {
            _bus = system.EventStream;
            //TODO: remove
            _bus.SetLogLevel(LogLevel.DebugLevel);
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
            if (!_bus.Subscribe(actor, messageType))
                return; //already subscribed
              

            List<IActorRef> subscribers;
            if (!Subscribers.TryGetValue(messageType, out subscribers))
            {
                subscribers = new List<IActorRef>();
                Subscribers[messageType] = subscribers;
            }
            subscribers.Add(actor);
        }
    }

    public class UnsuccessfullSubscribeException : Exception
    {
        public Type MessageType { get; }
        public IActorRef Actor { get;}

        public UnsuccessfullSubscribeException(Type messageType, IActorRef actor)
        {
            MessageType = messageType;
            Actor = actor;
        }
    }
}