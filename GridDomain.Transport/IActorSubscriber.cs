using System;
using Akka.Actor;

namespace GridDomain.Transport
{
    public interface IActorSubscriber
    {
        void Subscribe<TMessage>(IActorRef actor);
        void Unsubscribe(IActorRef actor, Type topic);
        void Subscribe(Type messageType, IActorRef actor, IActorRef subscribeNotificationWaiter = null);
    }
}