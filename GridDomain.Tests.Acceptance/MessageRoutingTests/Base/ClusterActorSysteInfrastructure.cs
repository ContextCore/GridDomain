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
        private ActorSystem[] _clusterSystems;

        public ClusterActorSysteInfrastructure(AkkaConfiguration conf):base(conf)
        {
            
        }


        public override void Init(IActorRef notifyActor)
        {
            base.Init(notifyActor);
            var  _clusterSystems = ActorSystemFactory.CreateCluster(AkkaConfig);
            this._clusterSystems = _clusterSystems;

            System = _clusterSystems.Last();
        }

        protected override void InitContainer(UnityContainer container, IActorRef actor)
        {
            base.InitContainer(container, actor);
            container.RegisterType<IHandlerActorTypeFactory, GridDomain.Tests.Acceptance.MessageRoutingTests>();

        }
    }
}