using System;
using System.Collections.Generic;
using Akka.Actor;
using Akka.Cluster.Tools.PublishSubscribe;
using Akka.Event;
using GridDomain.Common;
using GridDomain.Transport;

namespace GridDomain.Node.Cluster
{
    public interface ITopicExtractor
    {
        string GetTopic(object message);
    }
    
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

        public void Subscribe<TMessage>(IActorRef actor)
        {
            Subscribe(typeof(TMessage), actor, actor);
        }

        public void Unsubscribe(IActorRef actor, Type topic)
        {
            _transport.Ask<UnsubscribeAck>(new Unsubscribe(topic.FullName, actor), _timeout);
        }

        public void Subscribe(Type messageType, IActorRef actor, IActorRef subscribeNotificationWaiter)
        {
            var topic = _topicExtractor.GetTopic(messageType);
            
            if (!_subscribers.TryGetValue(topic, out var subcribers))
            {
                subcribers = new List<IActorRef>();
                _subscribers[topic] = subcribers;
            }
            subcribers.Add(actor);

            //TODO: replace wait with actor call
           var subscribe = _transport.Ask<SubscribeAck>(new Subscribe(topic, actor), _timeout).Result;
            subscribeNotificationWaiter?.Tell(subscribe);
           _log.Debug("Subscribing handler actor {Path} to topic {Topic}", actor.Path, topic);
        }

        public void Publish(object msg)
        {
            Publish(msg,MessageMetadata.Empty);
        }

        public void Publish(object msg, IMessageMetadata metadata)
        {
            var topic = _topicExtractor.GetTopic(msg);
            _transport.Tell(new Publish(topic, new MessageMetadataEnvelop(msg, metadata)));
        }
    }
}