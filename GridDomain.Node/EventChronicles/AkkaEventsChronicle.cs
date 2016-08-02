using System;
using System.Security.Policy;
using Akka;
using Akka.Actor;
using Akka.DI.Core;
using Akka.DI.Unity;
using CommonDomain.Core;
using GridDomain.CQRS.Messaging.Akka;
using GridDomain.CQRS.Messaging.MessageRouting;
using GridDomain.Node.Actors;
using GridDomain.Node.AkkaMessaging;
using GridDomain.Node.AkkaMessaging.Routing;
using GridDomain.Node.Configuration.Akka;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Node.Configuration.Persistence;
using GridDomain.Scheduling.Quartz;
using Microsoft.Practices.Unity;

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

            _system.AddDependencyResolver(new UnityDependencyResolver(new UnityContainer(), _system));

            _transport = new AkkaEventBusTransport(_system);

            var routingActor =
                _system.ActorOf(Props.Create(() => new LocalSystemRoutingActor(new DefaultHandlerActorTypeFactory(),new AkkaEventBusTransport(_system))));

            var actorLocator = new DefaultAggregateActorLocator();

            Router = new ActorMessagesRouter(routingActor,actorLocator);
        }

        public void Replay<TAggregate>(Guid aggregateId, Predicate<object> eventFilter = null) where TAggregate : AggregateBase
        {
            eventFilter = eventFilter ?? (o => true);

            var replayActor =
                _system.ActorOf(Props.Create(() => new EventsReplayActor<TAggregate>(_transport, eventFilter)),
                                                        AggregateActorName.New<TAggregate>(aggregateId).Name);

            replayActor.Ask<PlayFinished>(new Play());
        }
    }
}
