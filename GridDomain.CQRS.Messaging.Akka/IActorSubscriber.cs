using System;
using Akka.Actor;

namespace GridDomain.CQRS.Messaging.Akka
{
    public interface IActorSubscriber
    {
        void Subscribe<TMessage>(IActorRef actor);
        void Subscribe(Type messageType, IActorRef actor, IActorRef subscribeNotificationWaiter = null);
    }
}