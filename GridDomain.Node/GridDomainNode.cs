using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using Akka;
using Akka.Actor;
using Akka.DI.Core;
using Akka.DI.Unity;
using Akka.Monitoring;
using Akka.Monitoring.ApplicationInsights;
using Akka.Monitoring.PerformanceCounters;
using Akka.Serialization;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging;
using GridDomain.CQRS.Messaging.Akka;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Adapters;
using GridDomain.EventSourcing.VersionedTypeSerialization;
using GridDomain.Logging;
using GridDomain.Node.Actors;
using GridDomain.Node.AkkaMessaging.Routing;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Node.Configuration.Persistence;
using GridDomain.Scheduling.Akka.Messages;
using GridDomain.Scheduling.Integration;
using GridDomain.Scheduling.Quartz;
using Microsoft.Practices.Unity;
using IUnityContainer = Microsoft.Practices.Unity.IUnityContainer;

namespace GridDomain.Node
{
    public class GridDomainNode : IGridDomainNode
    {
        private static readonly IDictionary<TransportMode, Type> RoutingActorType = new Dictionary
            <TransportMode, Type>
        {
            {TransportMode.Standalone, typeof (LocalSystemRoutingActor)},
            {TransportMode.Cluster, typeof (ClusterSystemRouterActor)}
        };

        private readonly ISoloLogger _log = LogManager.GetLogger();
        private readonly IMessageRouteMap _messageRouting;
        private TransportMode _transportMode;
        public ActorSystem[] Systems;

        private Quartz.IScheduler _quartzScheduler;

        private readonly IContainerConfiguration _configuration;
        private readonly IQuartzConfig _quartzConfig;
        private readonly Func<ActorSystem[]> _actorSystemFactory;


        public IActorTransport Transport { get; private set; }
       // public MessagesListener Listener { get; private set; }

        public ActorSystem System;

        public GridDomainNode(IContainerConfiguration configuration,
                              IMessageRouteMap messageRouting,
                              Func<ActorSystem> actorSystemFactory) : this(configuration, messageRouting, () => new [] { actorSystemFactory()})
        {
        }

        public GridDomainNode(IContainerConfiguration configuration,
                              IMessageRouteMap messageRouting,
                              Func<ActorSystem[]> actorSystemFactory,
                              IQuartzConfig quartzConfig = null)
        {
            _actorSystemFactory = actorSystemFactory;
            _quartzConfig = quartzConfig ?? new InMemoryQuartzConfig();
            _configuration = configuration;
            _messageRouting = new CompositeRouteMap(messageRouting, 
                                                    new SchedulingRouteMap(),
                                                    new TransportMessageDumpMap()
                                                  );

            Container = new UnityContainer();
        }

        private void OnSystemTermination()
        {
            _log.Debug("grid node Actor system terminated");
        }
        private void OnSystemTermination(Task obj)
        {
            _log.Debug("grid node Actor system terminated");
        }

        public IUnityContainer Container { get; }

        public Guid Id { get; } = Guid.NewGuid();

        public void Start(IDbConfiguration databaseConfiguration)
        {
            Systems = _actorSystemFactory.Invoke();

           
            _transportMode = Systems.Length > 1 ? TransportMode.Cluster : TransportMode.Standalone;
            System = Systems.First();

            System.WhenTerminated.ContinueWith(OnSystemTermination);
            System.RegisterOnTermination(OnSystemTermination);
            System.AddDependencyResolver(new UnityDependencyResolver(Container, System));

            ConfigureContainer(Container, databaseConfiguration, _quartzConfig, System);

            var appInsightsConfig = Container.Resolve<IAppInsightsConfiguration>();
            var perfCountersConfig = Container.Resolve<IPerformanceCountersConfiguration>();

            if (appInsightsConfig.IsEnabled)
            {
                var monitor = new ActorAppInsightsMonitor(appInsightsConfig.Key);
                ActorMonitoringExtension.RegisterMonitor(System, monitor);
            }
            if (perfCountersConfig.IsEnabled)
            {
                ActorMonitoringExtension.RegisterMonitor(System, new ActorPerformanceCountersMonitor());
            }


            Transport = Container.Resolve<IActorTransport>();
            _quartzScheduler = Container.Resolve<Quartz.IScheduler>();
            _stopping = false;
            _log.Debug("Launching GridDomain node {Id}",Id);

            var props = System.DI().Props<GridNodeController>();
            var nodeController = System.ActorOf(props,nameof(GridNodeController));

            nodeController.Ask(new GridNodeController.Start
            {
                RoutingActorType = RoutingActorType[_transportMode]
            })
                .Wait(TimeSpan.FromSeconds(2));

            _log.Debug("GridDomain node {Id} started at home {Home}", Id, System.Settings.Home);

            _commandExecutor = new AkkaCommandExecutor(System,Transport);
        }

        private void ConfigureContainer(IUnityContainer unityContainer,
                                        IDbConfiguration databaseConfiguration, 
                                        IQuartzConfig quartzConfig, 
                                        ActorSystem actorSystem)
        {
            unityContainer.Register(new GridNodeContainerConfiguration(actorSystem,
                                                                       databaseConfiguration,
                                                                       _transportMode,
                                                                       quartzConfig));

            var persistentScheduler = System.ActorOf(System.DI().Props<SchedulingActor>(),nameof(SchedulingActor));
            unityContainer.RegisterInstance(new TypedMessageActor<ScheduleMessage>(persistentScheduler));
            unityContainer.RegisterInstance(new TypedMessageActor<ScheduleCommand>(persistentScheduler));
            unityContainer.RegisterInstance(new TypedMessageActor<Unschedule>(persistentScheduler));
            unityContainer.RegisterInstance(_messageRouting);

            _configuration.Register(unityContainer);
        }

        bool _stopping = false;
        private AkkaCommandExecutor _commandExecutor;
        public TimeSpan DefaultTimeout = TimeSpan.FromSeconds(10);

        public EventsAdaptersCatalog EventsAdaptersCatalog { get; } = AkkaDomainEventsAdapter.UpgradeChain;

        public void Stop()
        {
            if (_stopping) return;
            _stopping = true;

            _quartzScheduler.Shutdown(true);
            System.Terminate();
            System.Dispose();
            _log.Debug("GridDomain node {Id} stopped",Id);
        }

        public void Execute(params ICommand[] commands)
        {
            _commandExecutor.Execute(commands);
        }

        public Task<object> Execute(CommandPlan plan)
        {
            return _commandExecutor.Execute(plan);
        }

        public Task<T> Execute<T>(CommandPlan<T> plan)
        {
            return _commandExecutor.Execute(plan);
        }

        public IMessageWaiter<Task<IWaitResults>> NewWaiter(TimeSpan? defaultTimeout = null)
        {
            return new AkkaMessageLocalWaiter(System,Transport, defaultTimeout ?? DefaultTimeout);
        }

        public IMessageWaiter<IExpectedCommandExecutor> NewCommandWaiter(TimeSpan? defaultTimeout = null, bool failOnAnyFault = true)
        {
            return new AkkaCommandLocalWaiter(_commandExecutor, System, Transport, defaultTimeout ?? DefaultTimeout,failOnAnyFault);
        }
    }
}