using System;
using Akka.Actor;
using Akka.Cluster.Tools.PublishSubscribe;
using Akka.DI.Core;
using Akka.Routing;
using GridDomain.CQRS;
using GridDomain.Logging;
using NLog;

namespace GridDomain.Node.AkkaMessaging
{
    public class AkkaRoutingActor : TypedActor, IHandler<CreateRoute>
    {
        private Logger _log = LogManager.GetCurrentClassLogger();
        private IActorRef _distributedTransport;
        private readonly IHandlerActorTypeFactory _actorTypeFactory;
        //#  private HashSet<CreateRoute>  
        public AkkaRoutingActor(IHandlerActorTypeFactory actorTypeFactory)
        {
            _actorTypeFactory = actorTypeFactory;
        }

        protected override void PreStart()
        {
            _distributedTransport = DistributedPubSub.Get(Context.System).Mediator;
        }

        public void Handle(SubscribeAck msg)
        {
            _log.Trace($"Subscription was successfull for {msg.ToPropsString()}");
        }

        public void Handle(CreateRoute msg)
        {
            var handleActor = GetWorkerActorRef(msg);
            _log.Trace($"Created message handling actor for {msg.ToPropsString()}");
            var topic = msg.MessageType.FullName;

            _distributedTransport.Ask(new Subscribe(topic, handleActor)).Wait();

            _log.Trace($"Subscribed handler actor {handleActor.Path} to topic {topic}");
        }

        private IActorRef GetWorkerActorRef(CreateRoute msg)
        {

            var actorType = _actorTypeFactory.GetActorTypeFor(msg.MessageType, msg.HandlerType);
            var handleActorProps = Context.System.DI().Props(actorType);

            if (!string.IsNullOrEmpty(msg.MessageCorrelationProperty))
            {
                var router = new ConsistentHashingPool(Environment.ProcessorCount)
                    .WithHashMapping(m =>
                    {
                        var msgType = m.GetType();
                        if (msgType == msg.MessageType)
                        {
                            var value = msgType.GetProperty(msg.MessageCorrelationProperty)
                                               .GetValue(m);
                            return value;
                        }
                        return null;
                    });
                handleActorProps = handleActorProps.WithRouter(router);
            }

            var handleActor = Context.System.ActorOf(handleActorProps);
            return handleActor;
        }
    }
}