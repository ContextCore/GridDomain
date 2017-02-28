using System;
using Akka.Actor;

namespace GridDomain.Node.Actors
{
    public class GridNodeController : TypedActor
    {
        private readonly ActorMonitor _monitor;

        public GridNodeController()
        {
            _monitor = new ActorMonitor(Context);
        }

        public void Handle(Start msg)
        {
            _monitor.IncrementMessagesReceived();
            Sender.Tell(Started.Instance);
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

        public class Start {}

        public class Started
        {
            private Started() {}
            public static Started Instance { get; } = new Started();
        }
    }
}