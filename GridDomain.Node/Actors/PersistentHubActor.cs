using System;
using System.Collections.Generic;
using Akka.Actor;
using Akka.DI.Core;

namespace GridDomain.Node.Actors
{
    //TODO: think about replace with ConsistentHashingPool - need to deal with persistence 
    public abstract class PersistentHubActor: UntypedActor
    {
        private readonly IDictionary<Guid, IActorRef> _children = new Dictionary<Guid, IActorRef>();

        protected abstract string GetChildActorName(object message);
        protected abstract Guid GetChildActorId(object message);
        protected abstract Type GetChildActorType(object message);

        protected override void OnReceive(object message)
        {
            IActorRef knownChild;
            var childId = GetChildActorId(message);
            var name = GetChildActorName(message);

            if (!_children.TryGetValue(childId, out knownChild))
            {
                //TODO: Implement reuse logic

                var props = Context.DI().Props(GetChildActorType(message));
                knownChild = _children[childId] = Context.ActorOf(props, name);
            }

            knownChild.Tell(message);
        }
    }
}