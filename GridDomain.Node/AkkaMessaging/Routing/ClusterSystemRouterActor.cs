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
    public class ClusterSystemRouterActor : BusRoutingActor
    {
        private IActorRef _subscriptionWaiter;

        public ClusterSystemRouterActor(IHandlerActorTypeFactory actorTypeFactory,
            IActorSubscriber subscriber) : base(actorTypeFactory, subscriber)
        {
        }

        protected override RouterConfig CreateActorRouter(CreateActorRouteMessage msg)
        {
            //var router = new ClusterRouterPool(base.CreateActorRouter(msg), new ClusterRouterPoolSettings(10,10, true));
            return NoRouter.Instance;
        }

        public void Handle(SubscribeToRouteEstanblish msg)
        {
            _subscriptionWaiter = msg.Subscriber;
            Sender.Tell(new SubscribeToRouteEstanblishAck { Subscriber = msg.Subscriber });
        }


        public void Handle(SubscribeAck msg)
        {
            _subscriptionWaiter?.Tell(msg);
            Log.Trace(
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