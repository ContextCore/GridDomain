using System;
using System.Linq;
using Akka.Routing;
using GridDomain.CQRS.Messaging;
using GridDomain.CQRS.Messaging.Akka;

namespace GridDomain.Node.AkkaMessaging.Routing
{
    public class LocalSystemRoutingActor : RoutingActor
    {
        public LocalSystemRoutingActor(IHandlerActorTypeFactory actorTypeFactory, IActorSubscriber subscriber)
            : base(actorTypeFactory, subscriber)
        {
        }
    }
}