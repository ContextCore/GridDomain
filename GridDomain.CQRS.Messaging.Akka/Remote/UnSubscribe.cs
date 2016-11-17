using System;
using Akka.Actor;

namespace GridDomain.CQRS.Messaging.Akka.Remote
{
    public class UnSubscribe
    {
        public Type Topic { get; }
        public IActorRef Actor { get; }

        public UnSubscribe(IActorRef actor, Type topic)
        {
            Topic = topic;
            Actor = actor;
        }
    }
}