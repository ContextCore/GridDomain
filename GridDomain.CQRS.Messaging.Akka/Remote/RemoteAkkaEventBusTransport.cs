using System;
using Akka.Actor;

namespace GridDomain.CQRS.Messaging.Akka.Remote
{
    public class RemoteAkkaEventBusTransport : IActorTransport
    {
        private readonly IActorTransport _local;
        private readonly IActorRef _remoteSubscriber;

        public RemoteAkkaEventBusTransport(IActorTransport local,  IActorRef remoteSubscriber)
        {
            _remoteSubscriber = remoteSubscriber;
            _local = local;
        }

        public void Publish<T>(T msg)
        {
            _local.Publish(msg);
            _remoteSubscriber.Tell(new Publish(msg, typeof(T)));
        }

        public void Publish(object msg)
        {
            _local.Publish(msg);
            _remoteSubscriber.Tell(new Publish(msg,msg.GetType()));
        }

        public void Subscribe<TMessage>(IActorRef actor)
        {
            Subscribe(typeof(TMessage),actor);
        }

        public void Unsubscribe(IActorRef actor, Type topic)
        {
            _local.Unsubscribe(actor,topic);
            _remoteSubscriber.Tell(new UnSubscribe(actor, topic));
        }

        public void Subscribe(Type messageType, IActorRef actor, IActorRef subscribeNotificationWaiter = null)
        {
            _local.Subscribe(messageType, actor, subscribeNotificationWaiter);
            _remoteSubscriber.Tell(new Subscribe(actor, messageType,subscribeNotificationWaiter));
        }
    }
}