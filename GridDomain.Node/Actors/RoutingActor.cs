using System;
using System.Collections.Generic;
using System.Linq;
using Akka.Actor;
using Akka.DI.Core;
using Akka.Routing;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging.Akka;
using GridDomain.CQRS.Messaging.MessageRouting;
using GridDomain.EventSourcing;
using GridDomain.Logging;
using GridDomain.Node.AkkaMessaging;
using GridDomain.Node.AkkaMessaging.Routing;

namespace GridDomain.Node.Actors
{

    public class RouteCreated
    {
        public static RouteCreated Instance = new RouteCreated();
    }

    public abstract class RoutingActor : TypedActor, IHandler<CreateHandlerRouteMessage>,
                                                     IHandler<CreateActorRouteMessage>
    {
        private readonly IHandlerActorTypeFactory _actorTypeFactory;
        protected readonly ISoloLogger Log = LogManager.GetLogger();
        private readonly IActorSubscriber _subscriber;
        private readonly ActorMonitor _monitor;

        protected RoutingActor(IHandlerActorTypeFactory actorTypeFactory,
                               IActorSubscriber subscriber)
        {
            _subscriber = subscriber;
            _actorTypeFactory = actorTypeFactory;
            _monitor = new ActorMonitor(Context);
        }

        public void Handle(CreateActorRouteMessage msg)
        {
            _monitor.IncrementMessagesReceived();
            var handleActor = CreateActor(msg.ActorType, CreateActorRouter(msg), msg.ActorName);
            foreach (var msgRoute in msg.Routes)
            {
                Log.Info("Subscribed {actor} to {messageType}", handleActor.Path, msgRoute.MessageType);
                _subscriber.Subscribe(msgRoute.MessageType, handleActor, Self);
            }

            Sender.Tell(RouteCreated.Instance);
        }


        public void Handle(CreateHandlerRouteMessage msg)
        {
            _monitor.IncrementMessagesReceived();
            var msgType = CreateHandlerRouteMessage.GetTypeWithoutMetadata(msg.MessageType);

            var actorType = _actorTypeFactory.GetActorTypeFor(msgType, msg.HandlerType);
            string actorName = $"{msg.HandlerType}_for_{msg.MessageType.Name}";
           
            Self.Forward(new CreateActorRouteMessage(actorType,
                                                     actorName,
                                                     msg.PoolType,
                                                     new MessageRoute(msg.MessageType,msg.MessageCorrelationProperty)));
        }
        
        private static IEnumerable<Type> GetInterfacesAndBaseTypes(Type type)
        {
            yield return type;

            foreach (var interfaceType in type.GetInterfaces())
                yield return interfaceType;

            while ((type = type.BaseType) != null)
                yield return type;
        }

        protected virtual RouterConfig CreateActorRouter(CreateActorRouteMessage msg)
        {

            switch (msg.PoolKind)
            {
                case PoolKind.Random:
                    Log.Debug("Created random pool router to pass messages to {actor} ", msg.ActorName);
                    return new RandomPool(Environment.ProcessorCount);

                case PoolKind.ConsistentHash:
                    Log.Debug("Created consistent hashing pool router to pass messages to {actor} ", msg.ActorName);
                    var routesMap = msg.Routes.ToDictionary(r => r.MessageType, r => r.CorrelationField);
                    var pool = new ConsistentHashingPool(Environment.ProcessorCount,
                        message =>
                        {
                            var idContainer = (message as IMessageMetadataEnvelop)?.Message ?? message;

                            string prop = null;
                            if (GetInterfacesAndBaseTypes(message.GetType())
                                                       .Any(type => routesMap.TryGetValue(type, out prop)))
                            {
                                var value = idContainer.GetType()
                                                       .GetProperty(prop)
                                                       .GetValue(idContainer);
                                return value;
                            }
                            throw new ArgumentException($"Cannot find route for {message.GetType()}");
                        }
                        );
                    return pool;

                case PoolKind.None:
                    Log.Debug("Intentionally use {actor} without router", msg.ActorName);
                    return NoRouter.Instance;

                default:
                    Log.Debug("{actor} defaulted to no router", msg.ActorName);

                    return NoRouter.Instance;
            }
        }

        private IActorRef CreateActor(Type actorType, RouterConfig routeConfig, string actorName)
        {
            var handleActorProps = Context.System.DI().Props(actorType);
            handleActorProps = handleActorProps.WithRouter(routeConfig);

            var handleActor = Context.System.ActorOf(handleActorProps, actorName);
            return handleActor;
        }

        protected override void PreStart()
        {
            _monitor.IncrementActorStarted();
        }

        protected override void PostStop()
        {
            _monitor.IncrementActorStopped();
        }

        protected override void PreRestart(Exception reason, object message)
        {
            _monitor.IncrementActorRestarted();
        }
    }
}