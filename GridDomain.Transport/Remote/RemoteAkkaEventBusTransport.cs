using System;
using System.Linq;
using System.Threading.Tasks;
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

        public Task Subscribe<TMessage>(IActorRef actor)
        {
            return Subscribe(typeof(TMessage), actor);
        }

        public async Task Unsubscribe(IActorRef actor, Type topic)
        {
            await _local.Unsubscribe(actor, topic);
            await _remoteSubscriber.Ask<UnsubscribeAck>(new Unsubscribe(actor, topic), _timeout);
        }

        public async Task Subscribe(Type messageType, IActorRef actor, IActorRef subscribeNotificationWaiter = null)
        {
            await _local.Subscribe(messageType, actor, subscribeNotificationWaiter);
            await _remoteSubscriber.Ask<SubscribeAck>(new Subscribe(actor, messageType, subscribeNotificationWaiter));
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