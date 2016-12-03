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
        protected readonly ISoloLogger Logger = LogManager.GetLogger();
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
            Logger.Trace("Start clear childs process");
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

           Logger.Trace("Clear childs process finished, removed {childsToTerminate} childs", childsToTerminate.Length);
        }

        public class ClearChilds
        {
        }

        protected override void OnReceive(object msg)
        {
            _monitor.IncrementMessagesReceived();
            Logger.Trace("{ActorHub} received {@message}", Self.Path, msg);

            msg.Match()
               .With<ClearChilds>(m => Clear())
               .With<CheckHealth>(s => Sender.Tell(new HealthStatus(s.Payload)))
               .With<Terminated>(t => Logger.Trace("Child terminated: {path}",t.ActorRef.Path))
               .Default(message =>
                { 
                    ChildInfo knownChild;
                    var childId = GetChildActorId(message);
                    if (childId == Guid.Empty)
                        throw new InvalidChildIdException(message);

                    var name = GetChildActorName(message);

                    if (!Children.TryGetValue(childId, out knownChild))
                    {
                        //TODO: Implement reuse logic via selection
                        Logger.Trace("Creating child {childId} to process message {@message}", childId, msg);
                        var childActorType = GetChildActorType(message);

                        //TODO: think how to recover child create failure
                        var diActorContextAdapter = Context.DI();
                        var props = diActorContextAdapter.Props(childActorType);
                        var childActorRef = Context.ActorOf(props, name);
                        Context.Watch(childActorRef);
                        knownChild = new ChildInfo(childActorRef);
                        Children[childId] = knownChild;

                        Logger.Trace("Created new child {child} {id} from message {@message}", childActorType, childId, msg);
                    }

                    knownChild.LastTimeOfAccess = BusinessDateTime.UtcNow;
                    knownChild.ExpiresAt = knownChild.LastTimeOfAccess + ChildMaxInactiveTime;
                    knownChild.Ref.Tell(message);

                    Logger.Trace("Message {@msg} sent to child {id}", msg, childId);
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

    public class InvalidChildIdException : Exception
    {
        public object Message { get; }

        public InvalidChildIdException(object message):base("Child id should not be empty")
        {
            Message = message;
        }
    }
}