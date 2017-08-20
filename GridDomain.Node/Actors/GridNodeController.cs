using System;
using Akka.Actor;

namespace GridDomain.Node.Actors
{
    public class GridNodeController : ReceiveActor
    {
        private readonly ActorMonitor _monitor;

        public GridNodeController()
        {
            _monitor = new ActorMonitor(Context);
            Receive<Start>(m => {
                               _monitor.IncrementMessagesReceived();
                               Sender.Tell(Started.Instance);
                           });
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