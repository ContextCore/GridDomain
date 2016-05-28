using System;
using Akka.Actor;
using CommonDomain;
using CommonDomain.Core;
using CommonDomain.Persistence;
using CommonDomain.Persistence.EventStore;
using GridDomain.Balance;
using GridDomain.Balance.ReadModel;
using GridDomain.CQRS.ReadModel;
using GridDomain.EventSourcing;
using GridDomain.Node.AkkaMessaging;
using GridDomain.Node.Configuration;
using GridDomain.Node.DomainEventsPublishing;
using MemBus;
using Microsoft.Practices.Unity;
using NEventStore;
using NEventStore.Client;
using IPublisher = GridDomain.CQRS.Messaging.IPublisher;

namespace GridDomain.Node
{
    public static class CompositionRoot
    {

        public static void Init(IUnityContainer container,
                                ActorSystem actorSystem,
                                IDbConfiguration conf)
        {
            var transport = new AkkaEventBusTransport(actorSystem);
            
            container.RegisterInstance<IPublisher>(transport);
            container.RegisterInstance<IActorSubscriber>(transport);
            RegisterEventStore(container, conf);

            container.RegisterType<IHandlerActorTypeFactory, DefaultHandlerActorTypeFactory>();
            //TODO: replace dirty hack
            container.RegisterType<IAggregateActorLocator,DefaultAggregateActorLocator>();
        }

        public static void RegisterEventStore(IUnityContainer container, IDbConfiguration conf)
        {
            container.RegisterInstance(conf);
            var wireupEventStore = EventStoreSetup.WireupEventStore(conf.EventStoreConnectionString);
            container.RegisterInstance(wireupEventStore);
            container.RegisterType<IConstructAggregates, AggregateFactory>();
            container.RegisterType<IDetectConflicts, ConflictDetector>();
            container.RegisterType<IRepository, EventStoreRepository>();

        }

        public static PollingClient ConfigurePushingEventsFromStoreToBus(IStoreEvents eventStore,
            IObserver<ICommit> observer)
        {
            var pollingClient = new PollingClient(eventStore.Advanced, 10);
            var observeCommits = pollingClient.ObserveFrom(null);

            observeCommits.Subscribe(observer);
            observeCommits.Start();

            return pollingClient;
        }
    }
}