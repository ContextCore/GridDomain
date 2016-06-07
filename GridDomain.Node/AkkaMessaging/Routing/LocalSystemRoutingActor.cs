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

        protected override RouterConfig CreateActorRouter(CreateActorRoute msg)
        {
            var routesMap = msg.Routes.ToDictionary(r => r.MessageType, r => r.CorrelationField);

            var localPool =
                new ConsistentHashingPool(Environment.ProcessorCount)
                    .WithHashMapping(m =>
                    {
                        var type = m.GetType();
                        var prop = routesMap[type];
                        return type.GetProperty(prop).GetValue(m);
                    });

            return localPool;
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