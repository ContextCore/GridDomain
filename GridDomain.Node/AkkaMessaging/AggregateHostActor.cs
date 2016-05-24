using System;
using System.Collections.Generic;
using Akka.Actor;
using Akka.Cluster.Routing;
using Akka.DI.Core;
using Akka.Routing;
using CommonDomain.Core;

namespace GridDomain.Node.AkkaMessaging
{
    public class AggregateHostActor<TAggregate,TAggregateActor> : ReceiveActor
        where TAggregateActor : AggregateActor<TAggregate>
        where TAggregate: AggregateBase
    {
        protected IDictionary<Guid,IActorRef> Children;

        protected void TellTo<T>(T message, Guid correlationId)
        {
            IActorRef knownChild;
            if (!Children.TryGetValue(correlationId, out knownChild))
            {
                knownChild = Children[correlationId] = Context.ActorOf(Context.DI().Props<TAggregateActor>());
            }
            knownChild.Tell(message);
        }        
    }
}