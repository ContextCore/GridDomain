using Akka.Actor;
using Akka.Monitoring;
using Akka.Monitoring.Impl;
using GridDomain.Common;
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
        }

        private string GetCounterName(string metricName)
        {
            return $"{_context.System.Name}.{_actorGroupName}.{metricName}";
        }
        private void IncrementCounter(string akkaActorRestarts)
        {
            _context.IncrementCounter(GetCounterName(akkaActorRestarts));
        }

        public void IncrementMessagesReceived()
        {
            //_context.IncrementMessagesReceived();
            IncrementCounter(CounterNames.ActorRestarts);
        }

        public void IncrementActorRestarted()
        {
           // _context.IncrementActorRestart();
            IncrementCounter(CounterNames.ActorRestarts);
        }

        public void IncrementActorStopped()
        {
          //  _context.IncrementActorStopped();
            IncrementCounter(CounterNames.ActorsStopped);
        }

        public void IncrementActorStarted()
        {
            //_context.IncrementActorCreated();
            IncrementCounter(CounterNames.ActorsCreated);
        }
    }
}