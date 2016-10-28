using Akka.Actor;

namespace GridDomain.CQRS.Messaging.Akka
{
    public static class ActorSubscriberExtensions
    {
        public static void Unsubscribe<T>(this IActorSubscriber subscriber, IActorRef actor)
        {
            subscriber.Unsubscribe(actor,typeof(T));
        }
    }
}