using System;
using Akka.Actor;
using Akka.Cluster.Tools.PublishSubscribe;
using NLog;

namespace GridDomain.Node.AkkaMessaging
{
    public class DistributedPubSubSubscriber : IActorSubscriber
    {
        private IActorRef _transport;
        private readonly Logger _log = LogManager.GetCurrentClassLogger();

        public DistributedPubSubSubscriber(ActorSystem system)
        {
            _transport = DistributedPubSub.Get(system).Mediator;
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
    }
}