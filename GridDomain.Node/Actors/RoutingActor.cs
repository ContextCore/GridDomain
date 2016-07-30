using System;
using System.Linq;
using Akka.Actor;
using Akka.DI.Core;
using Akka.Monitoring;
using Akka.Monitoring.Impl;
using Akka.Routing;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging.Akka;
using GridDomain.CQRS.Messaging.MessageRouting;
using GridDomain.EventSourcing;
using GridDomain.Logging;
using GridDomain.Node.Actors;

namespace GridDomain.Node.AkkaMessaging.Routing
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
            _actorLogName = GetType().BeautyName();
        }

        public void Handle(CreateActorRouteMessage msg)
        {
            Context.IncrementMessagesReceived();
            var handleActor = CreateActor(msg.ActorType, CreateActorRouter(msg), msg.ActorName);
            foreach (var msgRoute in msg.Routes)
                _subscriber.Subscribe(msgRoute.MessageType, handleActor, Self);
        }

        public void Handle(CreateHandlerRouteMessage msg)
        {
            Context.IncrementMessagesReceived();
            var actorType = _actorTypeFactory.GetActorTypeFor(msg.MessageType, msg.HandlerType);
            string actorName = $"{msg.HandlerType}_for_{msg.MessageType.Name}";
            Self.Tell(new CreateActorRouteMessage(actorType,actorName,new MessageRoute(msg.MessageType,msg.MessageCorrelationProperty)));
        }

        protected virtual Pool CreateActorRouter(CreateActorRouteMessage msg)
        {
            var routesMap = msg.Routes.ToDictionary(r => r.Topic, r => r.CorrelationField);

            if(routesMap.All(r => r.Value == null))
                return new RandomPool(Environment.ProcessorCount);

            var pool =
                new ConsistentHashingPool(Environment.ProcessorCount)
                    .WithHashMapping(m =>
                    {
                        var type = m.GetType();
                        string prop = null;

                        if(routesMap.TryGetValue(type.FullName,out prop))
                            return type.GetProperty(prop).GetValue(m);

                        //TODO: refactor. Need to pass events to schedulingSaga
                        if (typeof(ICommandFault).IsAssignableFrom(type))
                        {
                            prop = routesMap[typeof(ICommandFault).FullName];
                            return typeof(ICommandFault).GetProperty(prop).GetValue(m);
                        }

                        if (typeof(DomainEvent).IsAssignableFrom(type))
                        {
                            prop = routesMap[typeof(DomainEvent).FullName];
                            return typeof(DomainEvent).GetProperty(prop).GetValue(m);
                        }


                        if (typeof(ICommand).IsAssignableFrom(type))
                        {
                            prop = routesMap[typeof(DomainEvent).FullName];
                            return typeof(DomainEvent).GetProperty(prop).GetValue(m);
                        }

                        throw new ArgumentException();
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

        private readonly string _actorLogName;

        protected override void PreStart()
        {
            Context.IncrementCounter($"{_actorLogName}.{CounterNames.ActorsCreated}");
        }

        protected override void PostStop()
        {
            Context.IncrementCounter($"{_actorLogName}.{CounterNames.ActorsStopped}");
        }
        protected override void PreRestart(Exception reason, object message)
        {
            Context.IncrementCounter($"{_actorLogName}.{CounterNames.ActorRestarts}");
        }
    }
}