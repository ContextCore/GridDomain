using System;
using System.Linq;
using Akka.Actor;
using GridDomain.Node;
using GridDomain.Node.AkkaMessaging;
using GridDomain.Node.Configuration;
using Microsoft.Practices.Unity;

namespace GridDomain.Tests.Acceptance.MessageRoutingTests
{
    class ClusterActorSysteInfrastructure : ActorSystemInfrastruture
    {
        private ActorSystem[] ClusterNodes;

        public ClusterActorSysteInfrastructure(AkkaConfiguration conf):base(conf)
        {
            
        }


        public override void Init(IActorRef notifyActor)
        {
            base.Init(notifyActor);
            ClusterNodes = ActorSystemFactory.CreateCluster(AkkaConfig);
            System = ClusterNodes.Last();
        }

        protected override void InitContainer(UnityContainer container, IActorRef actor)
        {
            base.InitContainer(container, actor);
            container.RegisterType<IHandlerActorTypeFactory, ClusterActorTypeFactory>();
        }

        public override void Dispose()
        {
            base.Dispose();
            foreach (var system in ClusterNodes)
            {
                system.Terminate();
                system.Dispose();
            }
        }
    }
}