using System;
using System.Linq;
using Akka.Actor;
using Akka.Cluster.Routing;
using Akka.Cluster.Tools.PublishSubscribe;
using Akka.Routing;
using GridDomain.CQRS.Messaging;
using GridDomain.CQRS.Messaging.Akka;
using GridDomain.Node.Actors;

namespace GridDomain.Node.AkkaMessaging.Routing
{
    public class ClusterSystemRouterActor : RoutingActor
    {
        private IActorRef _subscriptionWaiter;

        public ClusterSystemRouterActor(IHandlerActorTypeFactory actorTypeFactory,
            IActorSubscriber subscriber) : base(actorTypeFactory, subscriber)
        {
        }

        protected override Pool CreateActorRouter(CreateActorRouteMessage msg)
        {
            var router = new ClusterRouterPool(base.CreateActorRouter(msg), new ClusterRouterPoolSettings(10,10, true));
            return router;
        }

        public void Handle(SubscribeToRouteEstanblish msg)
        {
            _subscriptionWaiter = msg.Subscriber;
            Sender.Tell(new SubscribeToRouteEstanblishAck { Subscriber = msg.Subscriber });
        }


        public void Handle(SubscribeAck msg)
        {
            _subscriptionWaiter?.Tell(msg);
            _log.Trace(
                "Subscription was successfull for topic {Topic} group {Group} path {Path}",
                msg.Subscribe.Topic, msg.Subscribe.Group, msg.Subscribe.Ref.Path);
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