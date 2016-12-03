using System;
using System.Linq;
using Akka.Actor;
using Akka.DI.Core;
using Akka.Routing;
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
            Context.Watch(handleActor);

            foreach (var msgRoute in msg.Routes)
            {
                Log.Info("Subscribed {actor} to {messageType}", handleActor.Path, msgRoute.MessageType);
                _subscriber.Subscribe(msgRoute.MessageType, handleActor, Self);
            }
        }

        public void Handle(Terminated terminated)
        {
            Log.Warn("Actor involved in message routing terminated: {actor}", terminated.ActorRef);    
        }

        public void Handle(CreateHandlerRouteMessage msg)
        {
            _monitor.IncrementMessagesReceived();
            var actorType = _actorTypeFactory.GetActorTypeFor(msg.MessageType, msg.HandlerType);
            string actorName = $"{msg.HandlerType}_for_{msg.MessageType.Name}";
            Self.Tell(new CreateActorRouteMessage(actorType,actorName,new MessageRoute(msg.MessageType,msg.MessageCorrelationProperty)));
        }

        protected virtual Pool CreateActorRouter(CreateActorRouteMessage msg)
        {
            var routesMap = msg.Routes.ToDictionary(r => r.Topic, r => r.CorrelationField);

            if (routesMap.All(r => r.Value == null))
            {
                Log.Debug("Created random pool router to pass messages to {actor} ", msg.ActorName);

                return new RandomPool(Environment.ProcessorCount);
            }


            var pool = new ConsistentHashingPool(Environment.ProcessorCount,
                       m =>
                       {
                           var type = m.GetType();
                           string prop = null;

                           if (!routesMap.TryGetValue(type.FullName, out prop))
                                throw new ArgumentException();

                           var value = type.GetProperty(prop).GetValue(m);

                           LogManager.GetLogger().Trace("routed message {msg} by property {property} with value {value}", m, prop, value);
                           return value;
                       });

            if(pool.Resizer != null)
                throw new InvalidOperationException("Pools with resizer are not supported for now");
          
            Log.Debug("Created consistent hash pool router to pass messages to {actor} ", msg.ActorName);
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