using System;
using Akka.Actor;

namespace GridDomain.CQRS.Messaging.Akka
{
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