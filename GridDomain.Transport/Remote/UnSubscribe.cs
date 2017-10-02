using System;
using Akka.Actor;

namespace GridDomain.Transport.Remote
{
    public class Unsubscribe
    {
        public Unsubscribe(IActorRef actor, Type topic)
        {
            Topic = topic;
            Actor = actor;
        }

        public Type Topic { get; }
        public IActorRef Actor { get; }
    }
}