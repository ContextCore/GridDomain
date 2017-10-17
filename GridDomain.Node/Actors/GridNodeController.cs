using System;
using Akka.Actor;

namespace GridDomain.Node.Actors
{
    public class GridNodeController : ReceiveActor
    {
        private readonly ActorMonitor _monitor;

        public GridNodeController(IActorRef commandPipe, IActorRef transportProxy)
        {
            _monitor = new ActorMonitor(Context);
            Receive<HeartBeat>(m => {
                               _monitor.IncrementMessagesReceived();
                               Sender.Tell(Alive.Instance);
                           });
            Receive<Connect>(m => {
                                   _monitor.IncrementMessagesReceived();
                                   Sender.Tell(new Connected(commandPipe, transportProxy));
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

        public class HeartBeat
        {
            private HeartBeat() { }
            public static HeartBeat Instance { get; } = new HeartBeat();
        }

        public class Alive
        {
            private Alive() {}
            public static Alive Instance { get; } = new Alive();
        }

        public class Connect
        {
            private Connect() { }
            public static Connect Instance { get; } = new Connect();
        }

        public class Connected
        {
            public IActorRef PipeRef { get; }
            public IActorRef TransportProxy { get; }

            public Connected(IActorRef pipeRef, IActorRef transportProxy)
            {
                PipeRef = pipeRef;
                TransportProxy = transportProxy;
            }

        }
    }
}