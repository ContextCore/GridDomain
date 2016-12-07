using System;
using Akka.Actor;

namespace GridDomain.CQRS.Messaging.Akka.Remote
{
    public class Unsubscribe
    {
        public Type Topic { get; }
        public IActorRef Actor { get; }

        public Unsubscribe(IActorRef actor, Type topic)
        {
            Topic = topic;
            Actor = actor;
        }
    }
}