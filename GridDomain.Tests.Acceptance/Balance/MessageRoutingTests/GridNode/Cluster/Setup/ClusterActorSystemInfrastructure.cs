using System.Linq;
using Akka.Actor;
using GridDomain.CQRS.Messaging;
using GridDomain.Node;
using GridDomain.Node.AkkaMessaging;
using GridDomain.Node.Configuration;
using Microsoft.Practices.Unity;

namespace GridDomain.Tests.Acceptance.Balance.MessageRoutingTests.GridNode.Cluster.Setup
{
    class ClusterActorSystemInfrastructure : ActorSystemInfrastruture
    {
        public ActorSystem[] Nodes;
        private DistributedPubSubTransport _transport;
        public ClusterActorSystemInfrastructure(AkkaConfiguration conf):base(conf)
        {
            
        }

        protected override void InitContainer(UnityContainer container, IActorRef actor)
        {
            base.InitContainer(container, actor);
            container.RegisterType<IHandlerActorTypeFactory, ClusterActorTypeFactory>();
        }

        protected override IActorSubscriber Subscriber => _transport;
        protected override IPublisher Publisher => _transport;

        protected override ActorSystem CreateSystem(AkkaConfiguration conf)
        {
            Nodes = ActorSystemFactory.CreateCluster(AkkaConfig);
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