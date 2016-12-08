using System;
using Akka.Actor;
using Akka.DI.Core;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging;
using GridDomain.Node.AkkaMessaging.Waiting;

namespace GridDomain.Node.Actors
{
    public class GridNodeController : TypedActor
    {
        private readonly IMessageRouteMap _messageRouting;

        public GridNodeController(IMessageRouteMap messageRouting)
        {
            _messageRouting = messageRouting;
            _monitor = new ActorMonitor(Context);
        }

        public void Handle(Start msg)
        {
            _monitor.IncrementMessagesReceived();
            var system = Context.System;
            var routingActor = system.ActorOf(system.DI().Props(msg.RoutingActorType),msg.RoutingActorType.Name);

            var actorMessagesRouter = new ActorMessagesRouter(routingActor);
            _messageRouting.Register(actorMessagesRouter)
                           .ContinueWith(T => new Started()).PipeTo(Sender);
        }
      
        public class Start
        {
            public Type RoutingActorType;
        }

        public class Started
        {
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