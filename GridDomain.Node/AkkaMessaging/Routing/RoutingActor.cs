using System;
using System.Linq;
using Akka.Actor;
using Akka.DI.Core;
using Akka.Routing;
using GridDomain.CQRS;
using GridDomain.Logging;
using GridDomain.Node.Actors;
using NLog;

namespace GridDomain.Node.AkkaMessaging.Routing
{
    public abstract class RoutingActor : TypedActor, IHandler<CreateHandlerRoute>,
                                                     IHandler<CreateActorRoute>
    {
        private readonly IHandlerActorTypeFactory _actorTypeFactory;
        protected readonly Logger _log = LogManager.GetCurrentClassLogger();
        private readonly IActorSubscriber _subscriber;

        protected RoutingActor(IHandlerActorTypeFactory actorTypeFactory,
            IActorSubscriber subscriber)
        {
            _subscriber = subscriber;
            _actorTypeFactory = actorTypeFactory;
        }

        public void Handle(CreateActorRoute msg)
        {
            var handleActor = CreateActor(msg.ActorType, CreateActorRouter(msg), msg.ActorName);
            foreach (var msgRoute in msg.Routes)
                _subscriber.Subscribe(msgRoute.MessageType, handleActor, Self);
        }

        public void Handle(CreateHandlerRoute msg)
        {
            var actorType = _actorTypeFactory.GetActorTypeFor(msg.MessageType, msg.HandlerType);
            string actorName = $"{msg.HandlerType}_for_{msg.MessageType.Name}";
            Self.Tell(new CreateActorRoute(actorType,actorName,new MessageRoute(msg.MessageType,msg.MessageCorrelationProperty)));
        }

        protected virtual Pool CreateActorRouter(CreateActorRoute msg)
        {
            var routesMap = msg.Routes.ToDictionary(r => r.MessageType, r => r.CorrelationField);

            var pool =
                new ConsistentHashingPool(Environment.ProcessorCount)
                    .WithHashMapping(m =>
                    {
                        var type = m.GetType();
                        var prop = routesMap[type];
                        return type.GetProperty(prop).GetValue(m);
                    });

            return pool;
        }
        private IActorRef CreateActor(Type actorType, 
                                      RouterConfig routeConfig,
                                      string actorName)
        {
            var handleActorProps = Context.System.DI().Props(actorType);
            handleActorProps = handleActorProps.WithRouter(routeConfig);

            var handleActor = Context.System.ActorOf(handleActorProps, actorName);
            return handleActor;
        }
    }
}