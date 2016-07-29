using System;
using System.Collections.Generic;
using System.Linq;
using Akka.Actor;
using Akka.DI.Core;
using GridDomain.Common;

namespace GridDomain.Node.Actors
{
    //TODO: think about replace with ConsistentHashingPool - need to deal with persistence 
    /// <summary>
    /// Any child should be terminated by ShutdownRequest message
    /// </summary>
    public abstract class PersistentHubActor: UntypedActor
    {
        protected readonly IDictionary<Guid, ChildInfo> Children = new Dictionary<Guid, ChildInfo>();
        private readonly IPersistentChildsRecycleConfiguration _recycleConfiguration;
        //TODO: replace with more efficient implementation
        protected virtual TimeSpan ChildClearPeriod => _recycleConfiguration.ChildClearPeriod;
        protected virtual TimeSpan ChildMaxInactiveTime => _recycleConfiguration.ChildMaxInactiveTime;

        protected abstract string GetChildActorName(object message);
        protected abstract Guid GetChildActorId(object message);
        protected abstract Type GetChildActorType(object message);

        public PersistentHubActor(IPersistentChildsRecycleConfiguration recycleConfiguration)
        {
            _recycleConfiguration = recycleConfiguration;
        }

        protected override void PreStart()
        {
            Context.System.Scheduler.ScheduleTellRepeatedly(ChildClearPeriod, ChildClearPeriod, Self, new ClearChilds(), Self);
        }

        protected virtual void Clear()
        {
           var now = BusinessDateTime.UtcNow;
           var childsToTerminate = Children.Where(c => now - c.Value.LastTimeOfAccess > ChildMaxInactiveTime)
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
            if (message is ClearChilds)
            {
                Clear();
                return;
            }

            ChildInfo knownChild;
            var childId = GetChildActorId(message);
            var name = GetChildActorName(message);

            if (!Children.TryGetValue(childId, out knownChild))
            {
                //TODO: Implement reuse logic via selection

                var childActorType = GetChildActorType(message);

                //TODO: think how to recover child create failure
                var props = Context.DI().Props(childActorType);
                var childActorRef = Context.ActorOf(props, name);

                knownChild = new ChildInfo(childActorRef);
                Children[childId] = knownChild;
            }

            Children[childId].LastTimeOfAccess = DateTimeFacade.UtcNow;
            knownChild.Ref.Tell(message);
        }

        public class ClearChilds
        {
        }
    }

  
}