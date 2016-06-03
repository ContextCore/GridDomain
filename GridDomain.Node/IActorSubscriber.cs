using System;
using Akka.Actor;

namespace GridDomain.Node
{
    public interface IActorSubscriber
    {
        void Subscribe<TMessage>(IActorRef actor);
        void Subscribe(Type messageType, IActorRef actor, IActorRef subscribeNotificationWaiter);
    }
}