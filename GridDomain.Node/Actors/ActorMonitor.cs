using Akka.Actor;
using Akka.Monitoring;
using Akka.Monitoring.Impl;
using GridDomain.Node.AkkaMessaging;

namespace GridDomain.Node.Actors
{
    public class ActorMonitor
    {
        private readonly IActorContext _context;
        private readonly string _actorGroupName;

        public ActorMonitor(IActorContext context, string actorName = null)
        {
            _context = context;
            _actorGroupName = actorName ?? context.Props.Type.BeautyName();

            // var allValues = new string[]
            // {
            //         CounterNames.ActorsStopped,
            //         CounterNames.ActorRestarts,
            //         CounterNames.ActorsCreated,
            //         CounterNames.DeadLetters,
            //         CounterNames.DebugMessages,
            //         CounterNames.ErrorMessages,
            //         CounterNames.InfoMessages,
            //         CounterNames.ReceivedMessages,
            //         CounterNames.UnhandledMessages,
            //         CounterNames.WarningMessages,
            // };
        }

        private string GetCounterName(string counterName)
        {
            return $"{_context.System.Name}.{counterName}.{_actorGroupName}";
        }
        private void IncrementCounter(string akkaActorRestarts)
        {
            _context.IncrementCounter(GetCounterName(akkaActorRestarts));
        }

        public void IncrementMessagesReceived()
        {
            IncrementCounter(CounterNames.ActorRestarts);
        }

        public void IncrementActorRestarted()
        {
            IncrementCounter(CounterNames.ActorRestarts);
        }

        public void IncrementActorStopped()
        {
            IncrementCounter(CounterNames.ActorsStopped);
        }

        public void IncrementActorStarted()
        {
            IncrementCounter(CounterNames.ActorsCreated);
        }
    }
}