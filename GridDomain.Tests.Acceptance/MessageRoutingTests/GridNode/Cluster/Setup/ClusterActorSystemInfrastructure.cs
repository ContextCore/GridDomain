using System.Linq;
using Akka.Actor;
using Akka.DI.Core;
using GridDomain.CQRS.Messaging;
using GridDomain.CQRS.Messaging.Akka;
using GridDomain.Node;
using GridDomain.Node.AkkaMessaging;
using GridDomain.Node.AkkaMessaging.Routing;
using GridDomain.Node.Configuration;
using Microsoft.Practices.Unity;

namespace GridDomain.Tests.Acceptance.MessageRoutingTests.GridNode.Cluster.Setup
{
    internal class ClusterActorSystemInfrastructure : ActorSystemInfrastruture
    {
        private DistributedPubSubTransport _transport;
        public ActorSystem[] Nodes;

        public ClusterActorSystemInfrastructure(AkkaConfiguration conf) : base(conf)
        {
        }

        protected override IActorSubscriber Subscriber => _transport;
        protected override IPublisher Publisher => _transport;

        protected override IActorRef CreateRoutingActor(ActorSystem system)
        {
            return system.ActorOf(system.DI().Props<ClusterSystemRouterActor>(), nameof(ClusterSystemRouterActor));
        }

        protected override void InitContainer(UnityContainer container, IActorRef actor)
        {
            base.InitContainer(container, actor);
            container.RegisterType<IHandlerActorTypeFactory, ClusterActorTypeFactory>();
        }

        protected override ActorSystem CreateSystem(AkkaConfiguration conf)
        {
            Nodes = ActorSystemFactory.CreateCluster(AkkaConfig).NonSeedNodes;
            var actorSystem = Nodes.Last();
            _transport = new DistributedPubSubTransport(actorSystem);
            return actorSystem;
        }

        public override void Dispose()
        {
            Akka.Cluster.Cluster.Get(Nodes.First()).Shutdown();
            foreach (var actorSystem in Nodes)
            {
                actorSystem.Terminate();
                actorSystem.Dispose();
            }
        }
    }
}