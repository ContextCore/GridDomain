using Akka.Actor;
using GridDomain.CQRS.Messaging;
using GridDomain.Node;
using GridDomain.Node.AkkaMessaging;
using GridDomain.Node.Configuration;

namespace GridDomain.Tests.Acceptance.Balance.MessageRoutingTests.GridNode.SingleSystem.Setup
{
    public class SingleActorSystemInfrastructure : ActorSystemInfrastruture
    {
        private AkkaEventBusTransport _transport;
        public SingleActorSystemInfrastructure(AkkaConfiguration conf) : base(conf)
        {

        }

        protected override IActorSubscriber Subscriber => _transport;
        protected override IPublisher Publisher => _transport;

        protected override ActorSystem CreateSystem(AkkaConfiguration conf)
        {
            var actorSystem = ActorSystemFactory.CreateActorSystem(AkkaConfig);
            _transport = new AkkaEventBusTransport(actorSystem);
            return actorSystem;
        }
    }
}