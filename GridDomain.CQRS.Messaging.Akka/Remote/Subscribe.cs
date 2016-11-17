using System;
using Akka.Actor;

namespace GridDomain.CQRS.Messaging.Akka.Remote
{
    public class Subscribe
    {
        public IActorRef Actor { get; }
        public Type Topic { get; }
        public IActorRef Notificator { get; }

        public Subscribe(IActorRef actor, Type topic, IActorRef notificator)
        {
            Actor = actor;
            Topic = topic;
            Notificator = notificator;
        }
    }

    public class SubscribeAck
    {
        public static SubscribeAck Instance = new SubscribeAck();
        private SubscribeAck() { }
    }
}