using System.Linq;
using Akka.Actor;
using GridDomain.Node;
using GridDomain.Node.AkkaMessaging;
using GridDomain.Node.Configuration;
using Microsoft.Practices.Unity;

namespace GridDomain.Tests.Acceptance.MessageRoutingTests.GridNode.Cluster.Setup
{
    class ClusterActorSystemInfrastructure : ActorSystemInfrastruture
    {
        public ActorSystem[] Nodes;

        public ClusterActorSystemInfrastructure(AkkaConfiguration conf):base(conf)
        {
            
        }

        protected override void InitContainer(UnityContainer container, IActorRef actor)
        {
            base.InitContainer(container, actor);
            container.RegisterType<IHandlerActorTypeFactory, ClusterActorTypeFactory>();
        }

        protected override ActorSystem CreateSystem(AkkaConfiguration conf)
        {
            Nodes = ActorSystemFactory.CreateCluster(AkkaConfig);
            return Nodes.Last();
        }

        public override void Dispose()
        {
            base.Dispose();
            foreach (var system in Nodes)
            {
                system.Terminate();
                system.Dispose();
            }
        }
    }
}