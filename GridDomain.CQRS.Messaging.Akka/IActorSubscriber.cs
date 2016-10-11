using System;
using Akka.Actor;

namespace GridDomain.CQRS.Messaging.Akka
{
    public interface IActorSubscriber
    {
        void Subscribe<TMessage>(IActorRef actor);
        void Unsubscribe(IActorRef actor, Type topic);
        void Subscribe(Type messageType, IActorRef actor, IActorRef subscribeNotificationWaiter = null);
    }

    public static class ActorSubscriberExtensions
    {
        public static void Unsubscribe<T>(this IActorSubscriber subscriber, IActorRef actor)
        {
            subscriber.Unsubscribe(actor,typeof(T));
        }
    }
}