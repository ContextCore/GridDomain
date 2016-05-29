using System;
using Akka.Actor;
using Akka.Cluster.Tools.PublishSubscribe;
using GridDomain.CQRS.Messaging;
using GridDomain.Logging;
using NLog;

namespace GridDomain.Node.AkkaMessaging
{
    public class CannotGetDistributedPubSubException : Exception
    {
    }

    public class DistributedPubSubTransport : IActorSubscriber, IPublisher
    {
        private IActorRef _transport;
        private readonly Logger _log = LogManager.GetCurrentClassLogger();

        public DistributedPubSubTransport(ActorSystem system)
        {
             var distributedPubSub = DistributedPubSub.Get(system);
            if (distributedPubSub == null)
                throw new CannotGetDistributedPubSubException();
            _transport = distributedPubSub.Mediator;
        }

        public void Subscribe<TMessage>(IActorRef actor)
        {
            Subscribe(typeof (TMessage), actor);
        }

        public void Subscribe(Type messageType, IActorRef actor)
        {
            var topic = messageType.FullName;
            _transport.Ask(new Subscribe(topic, actor)).Wait();
            _log.Trace($"Subscribing handler actor {actor.Path} to topic {topic}");
        }

        public void Publish<T>(T msg)
        {
            var topic = msg.GetType().FullName;
            _log.Trace($"Publishing message {msg.ToPropsString()} to akka distributed pub sub with topic {topic}");
            _transport.Tell(new Publish(topic, msg));
        }
    }
}