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
        private ActorSystem _actorSystem;

        public SingleActorSystemInfrastructure(AkkaConfiguration conf) : base(conf)
        {

        }

        protected override IActorSubscriber Subscriber => _transport;
        protected override IPublisher Publisher => _transport;

        protected override ActorSystem CreateSystem(AkkaConfiguration conf)
        {
            _actorSystem = ActorSystemFactory.CreateActorSystem(AkkaConfig);
            _transport = new AkkaEventBusTransport(_actorSystem);
            return _actorSystem;
        }

        public override void Dispose()
        {
            _actorSystem.Terminate();
            _actorSystem.Dispose();
        }
    }
}