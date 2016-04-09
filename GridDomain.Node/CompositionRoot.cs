using System;
using Akka.Actor;
using CommonDomain;
using CommonDomain.Core;
using CommonDomain.Persistence;
using CommonDomain.Persistence.EventStore;
using GridDomain.Balance;
using GridDomain.Balance.ReadModel;
using GridDomain.CQRS.Messaging;
using GridDomain.EventSourcing;
using GridDomain.Node.AkkaMessaging;
using GridDomain.Node.Configuration;
using GridDomain.Node.DomainEventsPublishing;
using Microsoft.Practices.Unity;
using NEventStore;
using NEventStore.Client;

namespace GridDomain.Node
{
    public static class CompositionRoot
    {
        public static void Init(IUnityContainer container,
            //  IMessageTransport messageTransport,
            ActorSystem actorSystem,
            IDbConfiguration conf)
        {
            IPublisher publisher = new AkkaPublisher(actorSystem);
            container.RegisterInstance(publisher);
            RegisterEventStore(container, conf);
            container.RegisterInstance<Func<BusinessBalanceContext>>(
                () => new BusinessBalanceContext(conf.ReadModelConnectionString));


            container.RegisterType<IHandlerActorTypeFactory, DefaultHandlerActorTypeFactory>();
            //register all message handlers available to communicate
            //need to do it on plugin approach
            container.RegisterType<BalanceCommandsHandler>();
            container.RegisterType<BusinessCurrentBalanceProjectionBuilder>();
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