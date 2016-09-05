using System;
using System.Collections.Generic;
using System.Linq;
using Akka;
using Akka.Actor;
using Akka.DI.Core;
using Akka.Monitoring;
using GridDomain.Common;

namespace GridDomain.Node.Actors
{
    //TODO: think about replace with ConsistentHashingPool - need to deal with persistence 
    /// <summary>
    /// Any child should be terminated by ShutdownRequest message
    /// </summary>
    public abstract class PersistentHubActor: UntypedActor
    {
        internal readonly IDictionary<Guid, ChildInfo> Children = new Dictionary<Guid, ChildInfo>();
        private readonly IPersistentChildsRecycleConfiguration _recycleConfiguration;
        //TODO: replace with more efficient implementation
        internal virtual TimeSpan ChildClearPeriod => _recycleConfiguration.ChildClearPeriod;
        internal virtual TimeSpan ChildMaxInactiveTime => _recycleConfiguration.ChildMaxInactiveTime;

        protected abstract string GetChildActorName(object message);
        protected abstract Guid GetChildActorId(object message);
        protected abstract Type GetChildActorType(object message);

        public PersistentHubActor(IPersistentChildsRecycleConfiguration recycleConfiguration, string counterName)
        {
            _recycleConfiguration = recycleConfiguration;
            _monitor = new ActorMonitor(Context, $"Hub_{counterName}");
        }

        
        private void Clear()
        {
           var now = BusinessDateTime.UtcNow;
           var childsToTerminate = Children.Where(c => now > c.Value.ExpiresAt)
                                           .Select(ch => ch.Key).ToArray();

           foreach (var childId in childsToTerminate)
           {
                //TODO: wait for child termination
               Children[childId].Ref.Tell(new ShutdownRequest(childId));
               Children.Remove(childId);
           }
        }

        protected override void OnReceive(object message)
        {
            _monitor.IncrementMessagesReceived();

            if (message is ClearChilds)
            {
                Clear();
                return;
            }
            message.Match().With<CheckHealth>(s => Sender.Tell(new HealthStatus(s.Payload)));

            ChildInfo knownChild;
            var childId = GetChildActorId(message);
            var name = GetChildActorName(message);

            if (!Children.TryGetValue(childId, out knownChild))
            {
                //TODO: Implement reuse logic via selection

                var childActorType = GetChildActorType(message);

                //TODO: think how to recover child create failure
                var diActorContextAdapter = Context.DI();
                var props = diActorContextAdapter.Props(childActorType);
                var childActorRef = Context.ActorOf(props, name);

                knownChild = new ChildInfo(childActorRef);
                Children[childId] = knownChild;
            }

            knownChild.LastTimeOfAccess = DateTimeFacade.UtcNow;
            knownChild.ExpiresAt = knownChild.LastTimeOfAccess + ChildMaxInactiveTime;
            knownChild.Ref.Tell(message);
        }

        public class ClearChilds
        {
        }

        private readonly ActorMonitor _monitor;

        protected override void PreStart()
        {
            _monitor.IncrementActorStarted();
            Context.System.Scheduler.ScheduleTellRepeatedly(ChildClearPeriod, ChildClearPeriod, Self, new ClearChilds(), Self);
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