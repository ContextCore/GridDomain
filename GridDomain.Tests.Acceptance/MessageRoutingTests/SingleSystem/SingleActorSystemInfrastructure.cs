using Akka.Actor;
using GridDomain.Node;
using GridDomain.Node.Configuration;

namespace GridDomain.Tests.Acceptance.MessageRoutingTests
{
    class SingleActorSystemInfrastructure : ActorSystemInfrastruture
    {
        public SingleActorSystemInfrastructure(AkkaConfiguration conf) : base(conf)
        {

        }
        public override void Init(IActorRef actor)
        {
            System = ActorSystemFactory.CreateActorSystem(AkkaConfig);
            base.Init(actor);
        }
    }
}