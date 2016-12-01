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
    public abstract class RoutingActor : TypedActor, IHandler<CreateHandlerRouteMessage>,
                                                     IHandler<CreateActorRouteMessage>
    {
        private readonly IHandlerActorTypeFactory _actorTypeFactory;
        protected readonly ISoloLogger _log = LogManager.GetLogger();
        private readonly IActorSubscriber _subscriber;

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
                _subscriber.Subscribe(msgRoute.MessageType, handleActor, Self);
        }

        public void Handle(CreateHandlerRouteMessage msg)
        {
            _monitor.IncrementMessagesReceived();
            var actorType = _actorTypeFactory.GetActorTypeFor(msg.MessageType, msg.HandlerType);
            string actorName = $"{msg.HandlerType}_for_{msg.MessageType.Name}";
            Self.Tell(new CreateActorRouteMessage(actorType,actorName,new MessageRoute(msg.MessageType,msg.MessageCorrelationProperty)));
        }

        private static IEnumerable<Type> GetInterfacesAndBaseTypes(Type type)
        {
            yield return type;

            foreach (var interfaceType in type.GetInterfaces())
                yield return interfaceType;

            while ((type = type.BaseType) != null)
                yield return type;
        }
        protected virtual Pool CreateActorRouter(CreateActorRouteMessage msg)
        {
            var routesMap = msg.Routes.ToDictionary(r => r.MessageType, r => r.CorrelationField);

            if(routesMap.All(r => r.Value == null))
                return new RandomPool(Environment.ProcessorCount);

            var pool =
                new ConsistentHashingPool(Environment.ProcessorCount)
                    .WithHashMapping(message =>
                    {
                        var idContainer= (message as IMessageMetadataEnvelop)?.Message ?? message;

                        string prop = null;
                        if (GetInterfacesAndBaseTypes(message.GetType())
                                                    .Any(type => routesMap.TryGetValue(type, out prop)))
                        {
                            var value = idContainer.GetType()
                                                   .GetProperty(prop)
                                                   .GetValue(idContainer);
                            return value;
                        }

                        ////TODO: refactor. Need to pass events to schedulingSaga
                        //if (typeof(IFault).IsAssignableFrom(type))
                        //{
                        //    prop = routesMap[typeof(IFault).FullName];
                        //    return typeof(IFault).GetProperty(prop).GetValue(m);
                        //}
                        //
                        //if (typeof(DomainEvent).IsAssignableFrom(type))
                        //{
                        //    prop = routesMap[typeof(DomainEvent).FullName];
                        //    return typeof(DomainEvent).GetProperty(prop).GetValue(m);
                        //}
                        //
                        //if (typeof(ICommand).IsAssignableFrom(type))
                        //{
                        //    prop = routesMap[typeof(DomainEvent).FullName];
                        //    return typeof(DomainEvent).GetProperty(prop).GetValue(m);
                        //}

                        throw new ArgumentException($"Cannot find route for {message.GetType()}");
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

        private readonly ActorMonitor _monitor;

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