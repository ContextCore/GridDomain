using Akka.Actor;
using CommonDomain;
using CommonDomain.Core;
using CommonDomain.Persistence;
using CommonDomain.Persistence.EventStore;
using GridDomain.CQRS.Messaging;
using GridDomain.EventSourcing;
using GridDomain.Node.AkkaMessaging;
using GridDomain.Node.Configuration;
using Microsoft.Practices.Unity;

namespace GridDomain.Node
{
    //TODO: refactor to good config

    public static class CompositionRoot
    {
        public static void Init(IUnityContainer container,
            ActorSystem actorSystem,
            IDbConfiguration conf,
            TransportMode transportMode)
        {
            //TODO: replace with config

            if (transportMode == TransportMode.Standalone)
            {
                var transport = new AkkaEventBusTransport(actorSystem);
                container.RegisterInstance<IPublisher>(transport);
                container.RegisterInstance<IActorSubscriber>(transport);
            }
            if (transportMode == TransportMode.Cluster)
            {
                var transport = new DistributedPubSubTransport(actorSystem);
                container.RegisterInstance<IPublisher>(transport);
                container.RegisterInstance<IActorSubscriber>(transport);
            }

            //TODO: remove
            RegisterEventStore(container, conf);

            container.RegisterType<IHandlerActorTypeFactory, DefaultHandlerActorTypeFactory>();
            container.RegisterType<IAggregateActorLocator, DefaultAggregateActorLocator>();
        }

        public static void RegisterEventStore(IUnityContainer container, IDbConfiguration conf)
        {
            container.RegisterInstance(conf);
            container.RegisterType<IConstructAggregates, AggregateFactory>();
            container.RegisterType<IDetectConflicts, ConflictDetector>();
            container.RegisterType<IRepository, EventStoreRepository>();
        }
    }
}