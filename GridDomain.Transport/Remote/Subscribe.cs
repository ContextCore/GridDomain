using System;
using Akka.Actor;

namespace GridDomain.Transport.Remote
{
    public class Subscribe
    {
        public Subscribe(IActorRef actor, Type topic, IActorRef notificator)
        {
            Actor = actor;
            Topic = topic;
            Notificator = notificator;
        }

        public IActorRef Actor { get; }
        public Type Topic { get; }
        public IActorRef Notificator { get; }
    }
}