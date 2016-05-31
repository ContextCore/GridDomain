using System;
using Akka.Routing;

namespace GridDomain.Node.AkkaMessaging
{
    public class LocalSystemRoutingActor : AkkaRoutingActor
    {
        public LocalSystemRoutingActor(IHandlerActorTypeFactory actorTypeFactory, IActorSubscriber subscriber) : base(actorTypeFactory, subscriber)
        {
        }

        protected override RouterConfig CreateActorRouter(CreateActorRoute msg)
        {
            return DefaultRouter;
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