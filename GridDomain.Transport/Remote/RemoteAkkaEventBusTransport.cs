using System;
using System.Linq;
using Akka.Actor;
using GridDomain.Common;

namespace GridDomain.Transport.Remote
{
    public class RemoteAkkaEventBusTransport : IActorTransport
    {
        private readonly IActorTransport _local;
        private readonly IActorRef _remoteSubscriber;
        private readonly TimeSpan _timeout;

        public RemoteAkkaEventBusTransport(IActorTransport local, IActorRef remoteSubscriber, TimeSpan timeout)
        {
            _timeout = timeout;
            _remoteSubscriber = remoteSubscriber;
            _local = local;
        }

        public void Subscribe<TMessage>(IActorRef actor)
        {
            Subscribe(typeof(TMessage), actor);
        }

        public void Unsubscribe(IActorRef actor, Type topic)
        {
            _local.Unsubscribe(actor, topic);
            _remoteSubscriber.Ask<UnsubscribeAck>(new Unsubscribe(actor, topic), _timeout).Wait();
        }

        public void Subscribe(Type messageType, IActorRef actor, IActorRef subscribeNotificationWaiter = null)
        {
            _local.Subscribe(messageType, actor, subscribeNotificationWaiter);
            _remoteSubscriber.Ask<SubscribeAck>(new Subscribe(actor, messageType, subscribeNotificationWaiter)).Wait();
        }

        public void Publish(object msg)
        {
            _local.Publish(msg);
            _remoteSubscriber.Ask<PublishAck>(new Publish(msg), _timeout).Wait();
        }

        public void Publish(object msg, IMessageMetadata metadata)
        {
            _local.Publish(msg);
            _remoteSubscriber.Ask<PublishAck>(new Publish(new MessageMetadataEnvelop(msg, metadata)), _timeout).Wait();
        }

        public void Publish(params object[] messages)
        {
            _local.Publish(messages);

            var remotePublish = new PublishMany(messages.Select(m => new Publish(m, m.GetType())).ToArray());

            _remoteSubscriber.Ask<PublishManyAck>(remotePublish, _timeout).Wait();
        }
    }
}