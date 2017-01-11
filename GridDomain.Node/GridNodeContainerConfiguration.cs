using System;
using Akka.Actor;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging;
using GridDomain.CQRS.Messaging.Akka;
using GridDomain.Node.Actors;
using GridDomain.Node.AkkaMessaging;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Scheduling;
using GridDomain.Scheduling.Quartz;
using GridDomain.Scheduling.Quartz.Retry;
using Microsoft.Practices.Unity;

namespace GridDomain.Node
{
    internal class GridNodeContainerConfiguration : IContainerConfiguration
    {
        private readonly ActorSystem _actorSystem;
        private readonly TransportMode _transportMode;
        private readonly IQuartzConfig _config;

        public GridNodeContainerConfiguration(ActorSystem actorSystem,
                                             TransportMode transportMode,
                                             IQuartzConfig config)
        {
            _config = config;
            _transportMode = transportMode;
            _actorSystem = actorSystem;
        }

        public void Register(IUnityContainer container)
        {
            RegisterScheduler(container);

            //TODO: replace with config
            IActorTransport transport;
            switch (_transportMode)
            {
                case TransportMode.Standalone:
                    transport = new LocalAkkaEventBusTransport(_actorSystem);
                    break;
                case TransportMode.Cluster:
                    transport = new DistributedPubSubTransport(_actorSystem);
                    break;
                default:
                    throw new ArgumentException(nameof(_transportMode));
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

            container.RegisterInstance(_actorSystem);

            var executor = new AkkaCommandExecutor(_actorSystem, transport);
            container.RegisterType<ICommandExecutor, AkkaCommandExecutor>();
            var messageWaiterFactory = new MessageWaiterFactory(executor,_actorSystem,TimeSpan.FromSeconds(15),transport);
            container.RegisterInstance<IMessageWaiterFactory>(messageWaiterFactory);
            container.RegisterInstance<ICommandWaiterFactory>(messageWaiterFactory);
        }

        private void RegisterScheduler(IUnityContainer container)
        {
            container.Register(new QuartzSchedulerConfiguration(_config ?? new PersistedQuartzConfig()));
            container.RegisterInstance<IRetrySettings>(new InMemoryRetrySettings(5,
                                                                                 TimeSpan.FromMinutes(10),
                                                                                 new DefaultExceptionPolicy()));
        }
    }
}