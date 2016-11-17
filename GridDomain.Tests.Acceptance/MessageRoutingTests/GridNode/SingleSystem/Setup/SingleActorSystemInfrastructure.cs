using Akka.Actor;
using Akka.DI.Core;
using GridDomain.CQRS.Messaging;
using GridDomain.CQRS.Messaging.Akka;
using GridDomain.Node;
using GridDomain.Node.AkkaMessaging.Routing;
using GridDomain.Node.Configuration;
using GridDomain.Node.Configuration.Akka;

namespace GridDomain.Tests.Acceptance.MessageRoutingTests.GridNode.SingleSystem.Setup
{
    public class SingleActorSystemInfrastructure : ActorSystemInfrastruture
    {
        private ActorSystem _actorSystem;
        private LocalAkkaEventBusTransport _transport;

        public SingleActorSystemInfrastructure(AkkaConfiguration conf) : base(conf)
        {
        }

        protected override IActorSubscriber Subscriber => _transport;
        protected override IPublisher Publisher => _transport;

        protected override ActorSystem CreateSystem(AkkaConfiguration conf)
        {
            _actorSystem = AkkaConfig.CreateSystem();
            _transport = new LocalAkkaEventBusTransport(_actorSystem);
            return _actorSystem;
        }

        public override void Dispose()
        {
            _actorSystem.Terminate();
            _actorSystem.Dispose();
        }

        protected override IActorRef CreateRoutingActor(ActorSystem system)
        {
            return system.ActorOf(system.DI().Props<LocalSystemRoutingActor>(), nameof(LocalSystemRoutingActor));
        }
    }
}