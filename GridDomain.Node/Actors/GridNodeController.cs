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

        public GridNodeController()
        {
            _monitor = new ActorMonitor(Context);
        }

        public void Handle(Start msg)
        {
            _monitor.IncrementMessagesReceived();
            Sender.Tell(Started.Instance);
        }
      
        public class Start
        {}

        public class Started
        {
            private Started() { }
            public static Started Instance { get; } = new Started();
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