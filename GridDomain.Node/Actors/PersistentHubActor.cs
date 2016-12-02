using System;
using System.Collections.Generic;
using System.Linq;
using Akka;
using Akka.Actor;
using Akka.DI.Core;
using Akka.Monitoring;
using GridDomain.Common;
using GridDomain.Logging;

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
        private readonly ISoloLogger _logger = LogManager.GetLogger();
        private readonly ActorMonitor _monitor;

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
            _logger.Trace("Start clear childs process");
           var now = BusinessDateTime.UtcNow;
           var childsToTerminate = Children.Where(c => now > c.Value.ExpiresAt)
                                           .Select(ch => ch.Key)
                                           .ToArray();

           foreach (var childId in childsToTerminate)
           {
                //TODO: wait for child termination
               Children[childId].Ref.Tell(GracefullShutdownRequest.Instance);
               Children.Remove(childId);
           }

           _logger.Trace("Clear childs process finished, removed {childsToTerminate} childs", childsToTerminate.Length);
        }

        public class ClearChilds
        {
        }

        protected override void OnReceive(object msg)
        {
            _monitor.IncrementMessagesReceived();
            _logger.Trace("{Name} received {@message}", this.GetType().Name, msg);

            msg.Match()
               .With<ClearChilds>(m => Clear())
               .With<CheckHealth>(s => Sender.Tell(new HealthStatus(s.Payload)))
               .With<Terminated>(t => _logger.Trace("Child terminated: {path}",t.ActorRef.Path))
               .Default(message =>
                { 
                    ChildInfo knownChild;
                    var childId = GetChildActorId(message);
                    var name = GetChildActorName(message);

                    if (!Children.TryGetValue(childId, out knownChild))
                    {
                        //TODO: Implement reuse logic via selection
                        _logger.Trace("Creating child {childId} to process message {@message}", childId, msg);
                        var childActorType = GetChildActorType(message);

                        //TODO: think how to recover child create failure
                        var diActorContextAdapter = Context.DI();
                        var props = diActorContextAdapter.Props(childActorType);
                        var childActorRef = Context.ActorOf(props, name);
                        Context.Watch(childActorRef);
                        knownChild = new ChildInfo(childActorRef);
                        Children[childId] = knownChild;

                        _logger.Trace("Created new child {child} {id} from message {@message}", childActorType, childId, msg);
                    }

                    knownChild.LastTimeOfAccess = BusinessDateTime.UtcNow;
                    knownChild.ExpiresAt = knownChild.LastTimeOfAccess + ChildMaxInactiveTime;
                    knownChild.Ref.Tell(message);

                    _logger.Trace("Message {@msg} sent to child {id}", msg, childId);
                });
        }

        protected override void Unhandled(object message)
        {
            base.Unhandled(message);
        }

        protected override SupervisorStrategy SupervisorStrategy()
        {
            return new OneForOneStrategy( //or AllForOneStrategy
                10,
                TimeSpan.FromSeconds(3),
                x =>
                {
                //Maybe we consider ArithmeticException to not be application critical
                //so we just ignore the error and keep going.
                if (x is ArithmeticException) return Directive.Resume;

                //Error that we cannot recover from, stop the failing actor
                if (x is NotSupportedException) return Directive.Stop;

                //In all other cases, just restart the failing actor
                return Directive.Restart;
                });
        }

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