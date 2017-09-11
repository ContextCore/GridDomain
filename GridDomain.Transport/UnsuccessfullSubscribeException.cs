using System;
using Akka.Actor;

namespace GridDomain.Transport
{
    public class UnsuccessfullSubscribeException : Exception
    {
        public UnsuccessfullSubscribeException()
        {
            
        }

        public UnsuccessfullSubscribeException(Type messageType, IActorRef actor)
        {
            MessageType = messageType;
            Actor = actor;
        }

        public Type MessageType { get; }
        public IActorRef Actor { get; }
    }
}