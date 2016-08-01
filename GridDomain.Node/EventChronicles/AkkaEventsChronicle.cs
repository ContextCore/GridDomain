using System;
using Akka;
using Akka.Actor;
using CommonDomain.Core;
using GridDomain.CQRS.Messaging.Akka;
using GridDomain.CQRS.Messaging.MessageRouting;
using GridDomain.Node.Actors;
using GridDomain.Node.AkkaMessaging;
using GridDomain.Node.AkkaMessaging.Routing;
using GridDomain.Node.Configuration.Akka;

namespace GridDomain.Node.EventChronicles
{
    public class AkkaEventsChronicle : IEventsChronicle
    {
        private readonly ActorSystem _system;
        private readonly AkkaEventBusTransport _transport;
        public IMessagesRouter Router { get; }

        public AkkaEventsChronicle(AkkaConfiguration akkaConf)
        {
            _system = akkaConf.CreateSystem();

            _transport = new AkkaEventBusTransport(_system);

            var routingActor =
                _system.ActorOf(Props.Create<RoutingActor>(() => new LocalSystemRoutingActor(new DefaultHandlerActorTypeFactory(),new AkkaEventBusTransport(_system))));

            var actorLocator = new DefaultAggregateActorLocator();

            Router = new ActorMessagesRouter(routingActor,actorLocator);
        }

        public void Replay<TAggregate>(Guid aggregateId, Predicate<object> eventFilter) where TAggregate : AggregateBase
        {
            var replayActor =
                _system.ActorOf(Props.Create(() => new EventsReplayActor<TAggregate>(_transport, eventFilter)));

            replayActor.Ask<PlayFinished>(new Play());
        }
    }
}
