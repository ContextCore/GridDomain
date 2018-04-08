using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Cluster.Tools.PublishSubscribe;
using Akka.Event;
using GridDomain.Common;
using GridDomain.Transport;

namespace GridDomain.Node.Cluster.Transport
{
    public class DistributedPubSubTransport : IActorTransport
    {
        private readonly ILoggingAdapter _log;
        private readonly TimeSpan _timeout = TimeSpan.FromSeconds(5);
        private readonly IActorRef _transport;
        private readonly IDictionary<string, List<IActorRef>> _subscribers = new Dictionary<string, List<IActorRef>>();
        private readonly ITopicExtractor _topicExtractor;

        public DistributedPubSubTransport(ActorSystem system,ITopicExtractor topicExtractor)
        {
            _topicExtractor = topicExtractor;
            _log = system.Log;

            DistributedPubSub distributedPubSub;
            try
            {
                distributedPubSub = DistributedPubSub.Get(system);
            }
            catch (Exception ex)
            {
                throw new CannotGetDistributedPubSubException(ex);
            }
            if (distributedPubSub == null)
                throw new CannotGetDistributedPubSubException();

            _transport = distributedPubSub.Mediator;
        }

        public Task Subscribe<TMessage>(IActorRef actor)
        {
            return Subscribe(typeof(TMessage), actor, actor);
        }

        public Task Unsubscribe(IActorRef actor, Type topicType)
        {
           var topic = _topicExtractor.GetSubscribeTopic(topicType);
           return _transport.Ask<UnsubscribeAck>(new Unsubscribe(topic, actor), _timeout);
        }

        public Task Subscribe(Type messageType, IActorRef actor, IActorRef subscribeNotificationWaiter)
        {
            var topic = _topicExtractor.GetSubscribeTopic(messageType);
            
            if (!_subscribers.TryGetValue(topic, out var subcribers))
            {
                subcribers = new List<IActorRef>();
                _subscribers[topic] = subcribers;
            }
            subcribers.Add(actor);

            return _transport.Ask<SubscribeAck>(new Subscribe(topic, actor), _timeout);
        }

        public void Publish(object msg)
        {
            var topic = _topicExtractor.GetPublishTopic(msg);
            _transport.Tell(new Publish(topic, msg));
        }

        public void Publish(object msg, IMessageMetadata metadata)
        {
            Publish(new MessageMetadataEnvelop(msg,metadata));
        }
    }
}