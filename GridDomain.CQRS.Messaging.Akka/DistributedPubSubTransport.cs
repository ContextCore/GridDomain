using System;
using Akka.Actor;
using Akka.Cluster.Tools.PublishSubscribe;
using GridDomain.Logging;
using LogManager = GridDomain.Logging.LogManager;

namespace GridDomain.CQRS.Messaging.Akka
{
    public class DistributedPubSubTransport : IActorSubscriber, IPublisher
    {
        private readonly ISoloLogger _log = LogManager.GetLogger();
        private readonly IActorRef _transport;

        public DistributedPubSubTransport(ActorSystem system)
        {
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
            Subscribe(typeof (TMessage), actor, actor);
        }

        public void Subscribe(Type messageType, IActorRef actor, IActorRef subscribeNotificationWaiter)
        {
            var topic = messageType.FullName;
            _transport.Ask<SubscribeAck>(new Subscribe(topic, actor))
                .PipeTo(subscribeNotificationWaiter);

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