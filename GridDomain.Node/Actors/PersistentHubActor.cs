using System;
using System.Collections.Generic;
using Akka.Actor;
using Akka.DI.Core;
using CommonDomain.Core;

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
            var aggregateId = GetChildActorId(message);
            var name = GetChildActorName(message);

            if (!_children.TryGetValue(aggregateId, out knownChild))
            {
                //TODO: Implement reuse logic
                var props = Context.DI().Props(GetChildActorType(message));
                knownChild = _children[aggregateId] = Context.ActorOf(props, name);
            }
            knownChild.Tell(message);
        }
    }
}