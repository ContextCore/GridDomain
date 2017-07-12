using System.Threading.Tasks;
using Akka.Actor;
using Akka.Monitoring;
using Akka.Monitoring.Impl;
using GridDomain.Common;
using GridDomain.CQRS;

namespace GridDomain.Node.Actors
{


   //public static class CommandExecuturExtensions
   //{
   //    public static Task Execute(this ICommandExecutor executor, ICommand command)
   //    {
   //        
   //    }
   //}
    public class ActorMonitor
    {
        private readonly string _actorGroupName;
        private readonly IActorContext _context;

        public ActorMonitor(IActorContext context, string actorName = null)
        {
            _context = context;
            _actorGroupName = actorName ?? context.Props.Type.BeautyName();
        }

        private string GetCounterName(string metricName)
        {
            return $"{_context.System.Name}.{_actorGroupName}.{metricName}";
        }

        public void Increment(string counterName)
        {
            _context.IncrementCounter(GetCounterName(counterName));
        }
        public void Increment<T>()
        {
            Increment(typeof(T).Name);
        }

        public void IncrementMessagesReceived()
        {
            Increment(CounterNames.ReceivedMessages);
        }

        public void IncrementActorRestarted()
        {
            Increment(CounterNames.ActorRestarts);
        }

        public void IncrementActorStopped()
        {
            Increment(CounterNames.ActorsStopped);
        }

        public void IncrementActorStarted()
        {
            Increment(CounterNames.ActorsCreated);
        }
    }
}