using System;
using System.Configuration;
using Akka.Actor;
using CommonDomain.Persistence;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging;
using GridDomain.CQRS.Messaging.Akka;
using GridDomain.EventSourcing;
using GridDomain.Logging;
using GridDomain.Node.Actors;
using GridDomain.Node.AkkaMessaging;
using GridDomain.Node.Configuration;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Scheduling;
using GridDomain.Scheduling.Integration;
using GridDomain.Scheduling.Quartz;
using Microsoft.Practices.Unity;

namespace GridDomain.Node
{
   // [Obsolete("Use GridNodeConainerConfiguration instead")]
    //TODO: refactor to good config via IContainerConfiguration
    internal static class CompositionRoot
    {
    //TODO: refactor to good config via IContainerConfiguration
        public static void Init(IUnityContainer container,
                                ActorSystem actorSystem,
                                TransportMode transportMode,
                                IQuartzConfig config = null)
        {
            container.Register(new QuartzSchedulerConfiguration(config ?? new PersistedQuartzConfig()));
       
            //TODO: replace with config
            IActorTransport transport;
            switch (transportMode)
            {
                case TransportMode.Standalone:
                    transport = new LocalAkkaEventBusTransport(actorSystem);
                break;
                case TransportMode.Cluster:
                    transport = new DistributedPubSubTransport(actorSystem);
                break;
                default:
                    throw new ArgumentException(nameof(transportMode));
            }

            container.RegisterInstance<IPublisher>(transport);
            container.RegisterInstance<IActorSubscriber>(transport);
            container.RegisterInstance<IActorTransport>(transport);
            container.RegisterType<IHandlerActorTypeFactory, DefaultHandlerActorTypeFactory>();
            container.RegisterType<IAggregateActorLocator, DefaultAggregateActorLocator>();
            container.RegisterType<IPersistentChildsRecycleConfiguration, DefaultPersistentChildsRecycleConfiguration>();
            container.RegisterInstance<IAppInsightsConfiguration>(AppInsightsConfigSection.Default ??
                                                                  new DefaultAppInsightsConfiguration());
            container.RegisterInstance<IPerformanceCountersConfiguration>(PerformanceCountersConfigSection.Default ??
                                                                  new DefaultPerfCountersConfiguration());

            container.RegisterInstance(actorSystem);

            var executor = new AkkaCommandExecutor(actorSystem, transport);
            container.RegisterType<ICommandExecutor, AkkaCommandExecutor>();
            var messageWaiterFactory = new MessageWaiterFactory(executor,actorSystem,TimeSpan.FromSeconds(15),transport);
            container.RegisterInstance<IMessageWaiterFactory>(messageWaiterFactory);
            container.RegisterInstance<ICommandWaiterFactory>(messageWaiterFactory);

        }
    }

}