using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly Logger _log = LogManager.GetCurrentClassLogger();
        private IActorRef _distributedTransport;

        protected override void PreStart()
        {
            _distributedTransport = DistributedPubSub.Get(Context.System).Mediator;
        }

        //TODO: notify about subscription finish
        public void Handle(SubscribeAck msg)
        {
            _log.Trace($"Subscription was successfull for {msg.ToPropsString()}");
        }

        public void Handle(CreateRoute msg)
        {
            var handleActor = GetWorkerActorRef(msg);
            _log.Trace($"Created message handling actor for {msg.ToPropsString()}");

            foreach (var topic in msg.MessagesToRoute.Select(msgToRegister => msgToRegister.MessageType.FullName))
            {
                _distributedTransport.Tell(new Subscribe(topic, handleActor));
                _log.Trace($"Subscribing handler actor {handleActor.Path} to topic {topic}");
            }

        }

        private IActorRef GetWorkerActorRef(CreateRoute msg)
        {
           
            var handleActorProps = Context.System.DI().Props();

            var routeConfig = CreateRouter(msg);

            handleActorProps = handleActorProps.WithRouter(routeConfig);

            var handleActor = Context.System.ActorOf(handleActorProps);
            return handleActor;
        }

        private RouterConfig CreateRouter(CreateRoute routeConfigMessage)
        {
            if (!string.IsNullOrEmpty(routeConfigMessage.MessageCorrelationProperty))
            {
                var router = new ConsistentHashingPool(Environment.ProcessorCount)
                                .WithHashMapping(GetCorrelationPropertyFromMessage(routeConfigMessage));
                return router;
            }
            else
            {
                var router = new RandomPool(Environment.ProcessorCount);
                return router;
            }
        }

        private ConsistentHashMapping GetCorrelationPropertyFromMessage(CreateRoute routeConfigMessage)
        {
            return m =>
            {
                var msgType = m.GetType();
                if (msgType != routeConfigMessage.MessageType)
                {
                    _log.Trace($"Bad message type. Expected:{routeConfigMessage.MessageType}, got:{msgType}");
                    return null;
                }

                var value = msgType.GetProperty(routeConfigMessage.MessageCorrelationProperty)
                                   .GetValue(m);
                if (!(value is Guid))
                    throw new InvalidCorrelationPropertyValue(value);

                _log.Trace($"created correlation id for message {m.GetType()}: {value}");
                return value;
            };
        }
    }

    internal class InvalidCorrelationPropertyValue : Exception
    {
        public InvalidCorrelationPropertyValue(object value):base(value?.ToString())
        {
            
        }
    }
}