using System;
using System.Collections.Generic;
using System.Linq;
using Akka.Actor;
using Akka.DI.Core;
using GridDomain.Common;

namespace GridDomain.Node.Actors
{

    class ChildLifetime
    {
        public IActorRef Child;
        public DateTime LastTimeOfAccess;
    }

    //TODO: think about replace with ConsistentHashingPool - need to deal with persistence 
    public abstract class PersistentHubActor: UntypedActor
    {
        private readonly IDictionary<Guid, IActorRef> _children = new Dictionary<Guid, IActorRef>();
        //TODO: replace with more efficient implementation
        public readonly IDictionary<IActorRef, DateTime> ChildLastAccessTimes = new Dictionary<IActorRef, DateTime>();
        protected virtual TimeSpan ChildClearPeriod { get; } = TimeSpan.FromMinutes(1);
        protected virtual TimeSpan ChildMaxInactiveTime { get; } = TimeSpan.FromMinutes(30);

        protected abstract string GetChildActorName(object message);
        protected abstract Guid GetChildActorId(object message);
        protected abstract Type GetChildActorType(object message);


        protected override void PreStart()
        {
            Context.System.Scheduler.ScheduleTellRepeatedly(ChildClearPeriod, ChildClearPeriod, Self, new ClearChilds(), Self);
        }

        private void Clear()
        {
           var now = DateTimeFacade.UtcNow;
            var childsToTerminate = ChildLastAccessTimes.Where(c => now - c.Value > ChildMaxInactiveTime)
                                                         .Select(ch => ch.Key).ToArray();
            foreach (var child in childsToTerminate)
            {
                ChildLastAccessTimes.Remove(child);

            }
        }

        protected override void OnReceive(object message)
        {
           // if (message is ClearChilds)
           // {
           //     Clear();
           //     return;
           // }

            IActorRef knownChild;
            var childId = GetChildActorId(message);
            var name = GetChildActorName(message);

            if (!_children.TryGetValue(childId, out knownChild))
            {
                //TODO: Implement reuse logic

                var childActorType = GetChildActorType(message);
                var props = Context.DI().Props(childActorType);
                knownChild = _children[childId] = Context.ActorOf(props, name);
            }
            ChildLastAccessTimes[knownChild] = DateTime.UtcNow;
            knownChild.Tell(message);
        }
    }

    public class ClearChilds
    {
    }
}