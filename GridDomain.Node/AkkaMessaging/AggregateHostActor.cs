using System;
using System.Collections.Generic;
using Akka.Actor;
using Akka.Cluster.Routing;
using Akka.DI.Core;
using Akka.Routing;
using CommonDomain.Core;

namespace GridDomain.Node.AkkaMessaging
{
    public class AggregateHostActor<TAggregate> : ReceiveActor
        where TAggregate: AggregateBase
    {
        private readonly IDictionary<Guid,IActorRef> _children = new Dictionary<Guid, IActorRef>();

        protected void TellTo<T>(T message, Guid aggregateId)
        {
            IActorRef knownChild;
            if (!_children.TryGetValue(aggregateId, out knownChild))
            {
                var props = Context.DI().Props<AggregateActor<TAggregate>>();
                var name = AggregateActorName.New<TAggregate>(aggregateId);
                knownChild = _children[aggregateId] = Context.ActorOf(props, name.ToString());
            }
            knownChild.Tell(message);
        }        
    }
}