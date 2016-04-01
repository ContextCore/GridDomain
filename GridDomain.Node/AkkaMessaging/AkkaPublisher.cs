using Akka.Actor;
using Akka.Cluster.Tools.PublishSubscribe;
using GridDomain.CQRS.Messaging;
using GridDomain.Logging;
using NLog;

namespace GridDomain.Node.AkkaMessaging
{
    public class AkkaPublisher: IPublisher
    {
        private readonly IActorRef _publisherActor;
        private Logger _log = LogManager.GetCurrentClassLogger();
        public AkkaPublisher(ActorSystem system)
        {
            _publisherActor = DistributedPubSub.Get(system).Mediator;
        }

        public void Publish<T>(T msg)
        {
            var topic = msg.GetType().FullName;
            _log.Trace($"Publishing message {msg.ToPropsString()} to akka distributed pub sub with topic {topic}");
            _publisherActor.Tell(new Publish(topic,msg));
        }
    }
}