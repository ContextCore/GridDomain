using System;
using System.Linq;
using Akka.Cluster.Routing;
using Akka.Routing;

namespace GridDomain.Node.AkkaMessaging.Routing
{
    public class ClusterSystemRouterActor : AkkaRoutingActor
    {
        public ClusterSystemRouterActor(IHandlerActorTypeFactory actorTypeFactory,
            IActorSubscriber subscriber) : base(actorTypeFactory, subscriber)
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

            var router = new ClusterRouterPool(localPool, new ClusterRouterPoolSettings(10, true, 2));
            return router;
        }

        protected override RouterConfig CreateRouter(CreateHandlerRoute handlerRouteConfigMessage)
        {
            if (string.IsNullOrEmpty(handlerRouteConfigMessage.MessageCorrelationProperty))
                return DefaultRouter;

            var localPool = new ConsistentHashingPool(Environment.ProcessorCount)
                .WithHashMapping(GetCorrelationPropertyFromMessage(handlerRouteConfigMessage));

            var router = new ClusterRouterPool(localPool, new ClusterRouterPoolSettings(10, true, 2));
            return router;
        }
    }
}