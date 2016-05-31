using System;
using System.Collections.Generic;
using Akka.Actor;
using Akka.Cluster.Tools.PublishSubscribe;
using Akka.DI.Core;
using Akka.Routing;
using GridDomain.CQRS;
using GridDomain.Logging;
using NLog;

namespace GridDomain.Node.AkkaMessaging
{
    public abstract class AkkaRoutingActor : TypedActor, IHandler<CreateHandlerRoute>,
                                                         IHandler<CreateActorRoute>
    {
        private readonly Logger _log = LogManager.GetCurrentClassLogger();
        private readonly IHandlerActorTypeFactory _actorTypeFactory;
        private readonly IActorSubscriber _subscriber;

        protected AkkaRoutingActor(IHandlerActorTypeFactory actorTypeFactory,
                                   IActorSubscriber subscriber)
        {
            _subscriber = subscriber;
            _actorTypeFactory = actorTypeFactory;
        }

        protected readonly RouterConfig DefaultRouter = new RandomPool(Environment.ProcessorCount);

        public void Handle(SubscribeAck msg)
        {
            _log.Trace($"Subscription was successfull for {msg.ToPropsString()}");
        }

        public void Handle(CreateHandlerRoute msg)
        {
            var actorType = _actorTypeFactory.GetActorTypeFor(msg.MessageType, msg.HandlerType);
            var handleActor = CreateHandleActor(msg, actorType, CreateRouter);
            _log.Trace($"Created message handling actor for {msg.ToPropsString()}");

            _subscriber.Subscribe(msg.MessageType, handleActor);
        }

        public void Handle(CreateActorRoute msg)
        {
            var aggregateActorOpenType = typeof (AggregateHostActor<>);
            var actorType = aggregateActorOpenType.MakeGenericType(msg.AggregateType);
            var handleActor = CreateHandleActor(msg, actorType, CreateActorRouter);

            foreach(var msgRoute in msg.Routes)
                _subscriber.Subscribe(msgRoute.MessageType, handleActor);
        }

        protected abstract RouterConfig CreateActorRouter(CreateActorRoute msg);
        protected abstract RouterConfig CreateRouter(CreateHandlerRoute handlerRouteConfigMessage);

        private IActorRef CreateHandleActor<TMessage>(TMessage msg, Type actorType, Func<TMessage, RouterConfig> routerFactory)
        {
            var handleActorProps = Context.System.DI().Props(actorType);
            var routeConfig = routerFactory(msg);
            handleActorProps = handleActorProps.WithRouter(routeConfig);
            var handleActor = Context.System.ActorOf(handleActorProps);
            return handleActor;
        }

        protected ConsistentHashMapping GetCorrelationPropertyFromMessage(CreateHandlerRoute handlerRouteConfigMessage)
        {
            return m =>
            {
                var msgType = m.GetType();
                if (msgType != handlerRouteConfigMessage.MessageType)
                {
                    _log.Trace($"Bad message type. Expected:{handlerRouteConfigMessage.MessageType}, got:{msgType}");
                    return null;
                }

                var value = msgType.GetProperty(handlerRouteConfigMessage.MessageCorrelationProperty)
                                   .GetValue(m);
                if (!(value is Guid))
                    throw new InvalidCorrelationPropertyValue(value);

                _log.Trace($"created correlation id for message {m.GetType()}: {value}");
                return value;
            };
        }
    }
}