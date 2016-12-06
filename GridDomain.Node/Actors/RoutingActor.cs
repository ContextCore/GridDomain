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
            //Context.Watch(handleActor);

            foreach (var msgRoute in msg.Routes)
            {
                Log.Info("Subscribed {actor} to {messageType}", handleActor.Path, msgRoute.MessageType);
                _subscriber.Subscribe(msgRoute.MessageType, handleActor, Self);
            }
        }

      //  public void Handle(Terminated terminated)
      //  {
      //      Log.Warn("Actor involved in message routing terminated: {actor}", terminated.ActorRef.Path);    
      //  }

        public void Handle(CreateHandlerRouteMessage msg)
        {
            _monitor.IncrementMessagesReceived();
            var actorType = _actorTypeFactory.GetActorTypeFor(msg.MessageType, msg.HandlerType);
            string actorName = $"{msg.HandlerType}_for_{msg.MessageType.Name}";
           
            Self.Tell(new CreateActorRouteMessage(actorType,
                                                  actorName,
                                                  msg.PoolType,
                                                  new MessageRoute(msg.MessageType,msg.MessageCorrelationProperty)));
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
                    var routesMap = msg.Routes.ToDictionary(r => r.Topic, r => r.CorrelationField);
                    var pool = new ConsistentHashingPool(Environment.ProcessorCount,
                        m =>
                        {
                            var type = m.GetType();
                            string prop = null;

                            if (!routesMap.TryGetValue(type.FullName, out prop))
                                throw new CannotFindRouteException(m);

                            var value = type.GetProperty(prop).GetValue(m);

                            return value;
                        });
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

    public class CannotFindRouteException : Exception
    {
        public object Msg { get; }

        public CannotFindRouteException(object msg)
        {
            Msg = msg;
        }
    }
}