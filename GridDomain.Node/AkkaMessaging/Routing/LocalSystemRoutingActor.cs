using System;
using System.Linq;
using Akka.Routing;

namespace GridDomain.Node.AkkaMessaging.Routing
{
    public class LocalSystemRoutingActor : AkkaRoutingActor
    {
        public LocalSystemRoutingActor(IHandlerActorTypeFactory actorTypeFactory, IActorSubscriber subscriber)
            : base(actorTypeFactory, subscriber)
        {
        }

        protected override RouterConfig CreateRouter(CreateHandlerRoute handlerRouteConfigMessage)
        {
            if (string.IsNullOrEmpty(handlerRouteConfigMessage.MessageCorrelationProperty))
                return DefaultRouter;

            var router =
                new ConsistentHashingPool(Environment.ProcessorCount)
                    .WithHashMapping(GetCorrelationPropertyFromMessage(handlerRouteConfigMessage));

            return router;
        }
    }
}