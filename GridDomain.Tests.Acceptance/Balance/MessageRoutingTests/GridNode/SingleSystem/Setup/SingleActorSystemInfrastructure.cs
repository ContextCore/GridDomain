using Akka.Actor;
using GridDomain.Node;
using GridDomain.Node.Configuration;

namespace GridDomain.Tests.Acceptance.Balance.MessageRoutingTests.GridNode.SingleSystem.Setup
{
    public class SingleActorSystemInfrastructure : ActorSystemInfrastruture
    {
        public SingleActorSystemInfrastructure(AkkaConfiguration conf) : base(conf)
        {

        }
  
        protected override ActorSystem CreateSystem(AkkaConfiguration conf)
        {
            return ActorSystemFactory.CreateActorSystem(AkkaConfig);
        }
    }
}