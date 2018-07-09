using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Akka.Actor;
using GridDomain.Common;
using GridDomain.CQRS;

namespace GridDomain.Node.Actors
{

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
          //  _context.IncrementCounter(GetCounterName(counterName));
        }
        
        public void Timing(string counterName,long time, double sampleRate = 1D)
        {
           // _context.Timing(GetCounterName(counterName), time, sampleRate);
        }

        public interface ITimer
        {
            void Stop();
        }

        private class StopWatchTimer : ITimer
        {
            private readonly ActorMonitor _actorMonitor;
            private readonly string _name;
            private readonly Stopwatch watch = new Stopwatch();
            
            public StopWatchTimer(string name, ActorMonitor monitor)
            {
                _name = name;
                _actorMonitor = monitor;
            }

            public void Start()
            {
                watch.Start();
            }
            
            public void Restart()
            {
                watch.Restart();
            }
            
            public void Stop()
            {
                watch.Stop();
                _actorMonitor.Timing(_name,watch.ElapsedMilliseconds);
            }
        }
        private readonly IDictionary<string,StopWatchTimer> _timersCache = new Dictionary<string, StopWatchTimer>();
        
        public ITimer StartMeasureTime(string counterName)
        {
            if (_timersCache.TryGetValue(counterName, out var timer))
            {
                timer.Restart();
                return timer;
            }
           
            timer = new StopWatchTimer(counterName,this);
            _timersCache[counterName] = timer;
            timer.Start();
            return timer;
        }

        public void Gauge(string name)
        {
           // _context.Gauge(GetCounterName(name));
        }
        
        public void Increment<T>()
        {
            Increment(typeof(T).Name);
        }

        public void IncrementMessagesReceived()
        {
            Increment("ReceivedMessages");
        }

        public void IncrementActorRestarted()
        {
            Increment("ActorRestarts");
        }

        public void IncrementActorStopped()
        {
            Increment("ActorsStopped");
        }

        public void IncrementActorStarted()
        {
            Increment("ActorsCreated");
        }
    }
}