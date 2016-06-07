using System;
using System.Linq;
using Akka.Actor;
using Akka.Cluster.Routing;
using Akka.Cluster.Tools.PublishSubscribe;
using Akka.Routing;

namespace GridDomain.Node.AkkaMessaging.Routing
{
    public class ClusterSystemRouterActor : RoutingActor
    {
        private IActorRef _subscriptionWaiter;

        public ClusterSystemRouterActor(IHandlerActorTypeFactory actorTypeFactory,
            IActorSubscriber subscriber) : base(actorTypeFactory, subscriber)
        {
        }

        protected override Pool CreateActorRouter(CreateActorRoute msg)
        {
            var router = new ClusterRouterPool(base.CreateActorRouter(msg), new ClusterRouterPoolSettings(10, true, 2));
            return router;
        }

        public void Handle(SubscribeToRouteEstanblish msg)
        {
            _subscriptionWaiter = msg.Subscriber;
            Sender.Tell(new SubscribeToRouteEstanblishAck {Subscriber = msg.Subscriber});
        }


        public void Handle(SubscribeAck msg)
        {
            _subscriptionWaiter?.Tell(msg);
            _log.Trace(
                $"Subscription was successfull for topic {msg.Subscribe.Topic} group {msg.Subscribe.Group} path {msg.Subscribe.Ref.Path}");
        }

        public class SubscribeToRouteEstanblishAck
        {
            public IActorRef Subscriber;
        }

        public class SubscribeToRouteEstanblish
        {
            public IActorRef Subscriber;
        }
    }
}