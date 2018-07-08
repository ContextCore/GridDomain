using System;
using System.Threading.Tasks;
using Akka.Actor;

namespace GridDomain.Transport
{
    public interface IActorSubscriber
    {
        Task Subscribe<TMessage>(IActorRef actor);
        Task Unsubscribe(IActorRef actor, Type topic);
        Task Subscribe(Type messageType, IActorRef actor, IActorRef subscribeNotificationWaiter = null);
    }
}