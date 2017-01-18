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
using CommonDomain.Persistence;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging;
using GridDomain.CQRS.Messaging.Akka;
using GridDomain.CQRS.Messaging.Akka.Remote;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Adapters;
using GridDomain.EventSourcing.VersionedTypeSerialization;
using GridDomain.Logging;
using GridDomain.Node.Actors;
using GridDomain.Node.AkkaMessaging.Routing;
using GridDomain.Node.Configuration.Composition;
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
            {TransportMode.Standalone, typeof (LocalSystemBusRoutingActor)},
            {TransportMode.Cluster, typeof (ClusterSystemRouterActor)}
        };

        private readonly ILogger _log = LogManager.GetLogger();
        private readonly IMessageRouteMap _messageRouting;
        private TransportMode _transportMode;
        public ActorSystem[] Systems;

        private Quartz.IScheduler _quartzScheduler;

        private readonly IContainerConfiguration _configuration;
        private readonly IQuartzConfig _quartzConfig;
        private readonly Func<ActorSystem[]> _actorSystemFactory;

        bool _stopping = false;
        public TimeSpan DefaultTimeout { get; }
        private IMessageWaiterFactory _waiterFactory;
        private ICommandExecutor _commandExecutor;


        public EventsAdaptersCatalog EventsAdaptersCatalog { get; } = new EventsAdaptersCatalog();
        public AggregatesSnapshotsFactory AggregateFromSnapshotsFactory { get; } = new AggregatesSnapshotsFactory();
        public IActorTransport Transport { get; private set; }

        public ActorSystem System { get; private set; }
        public IActorRef EventBusForwarder { get; private set; }

        public GridDomainNode(IContainerConfiguration configuration,
                              IMessageRouteMap messageRouting,
                              Func<ActorSystem> actorSystemFactory,
                              TimeSpan? defaultTimeout =null) : this(configuration, messageRouting, () => new [] { actorSystemFactory()}, null, defaultTimeout)
        {
        }

        public GridDomainNode(IContainerConfiguration configuration,
                              IMessageRouteMap messageRouting,
                              Func<ActorSystem[]> actorSystemFactory,
                              IQuartzConfig quartzConfig = null, 
                              TimeSpan? defaultTimeout = null)
        {
            _actorSystemFactory = actorSystemFactory;
            _quartzConfig = quartzConfig ?? new InMemoryQuartzConfig();
            _configuration = configuration;
            _messageRouting = new CompositeRouteMap(messageRouting);
            DefaultTimeout = defaultTimeout ?? TimeSpan.FromSeconds(10);
        }

        private void OnSystemTermination()
        {
            _log.Debug("grid node Actor system terminated");
        }

        public IUnityContainer Container { get; private set; }

        public Guid Id { get; } = Guid.NewGuid();

        public async Task Start()
        {
            _stopping = false;

            Container = new UnityContainer();
            Systems = _actorSystemFactory.Invoke();
            System = Systems.First();

            System.InitDomainEventsSerialization(EventsAdaptersCatalog);

            _transportMode = Systems.Length > 1 ? TransportMode.Cluster : TransportMode.Standalone;
            System.RegisterOnTermination(OnSystemTermination);
            System.AddDependencyResolver(new UnityDependencyResolver(Container, System));

            await ConfigureContainer(Container, _quartzConfig, System);

            Transport = Container.Resolve<IActorTransport>();
            _quartzScheduler = Container.Resolve<Quartz.IScheduler>();
            _commandExecutor = Container.Resolve<ICommandExecutor>();
            _waiterFactory = Container.Resolve<IMessageWaiterFactory>();

            EventBusForwarder = System.ActorOf(Props.Create(() => new EventBusForwarder(Transport)),nameof(EventBusForwarder));
            var appInsightsConfig = Container.Resolve<IAppInsightsConfiguration>();
            var perfCountersConfig = Container.Resolve<IPerformanceCountersConfiguration>();

            RegisterCustomAggregateSnapshots();

            if (appInsightsConfig.IsEnabled)
            {
                var monitor = new ActorAppInsightsMonitor(appInsightsConfig.Key);
                ActorMonitoringExtension.RegisterMonitor(System, monitor);
            }
            if (perfCountersConfig.IsEnabled)
            {
                ActorMonitoringExtension.RegisterMonitor(System, new ActorPerformanceCountersMonitor());
            }

            _log.Debug("Launching GridDomain node {Id}",Id);


            var props = System.DI().Props<GridNodeController>();
            var nodeController = System.ActorOf(props,nameof(GridNodeController));

            await nodeController.Ask<GridNodeController.Started>(new GridNodeController.Start
            {
                RoutingActorType = RoutingActorType[_transportMode]
            });

            _log.Debug("GridDomain node {Id} started at home {Home}", Id, System.Settings.Home);
        }

        private async Task<ICommandExecutor> ConfigureCommandPipe()
        {
            var pipeBuilder = new CommandPipeBuilder(System, Container);
            var commandExecutorActor = await pipeBuilder.Init();
            await _messageRouting.Register(pipeBuilder);

            return new AkkaCommandPipeExecutor(System, Transport, commandExecutorActor,DefaultTimeout);
        }

        private void RegisterCustomAggregateSnapshots()
        {
            var factories = Container.ResolveAll(typeof(IConstructAggregates))
                .Select(o => new {Type = o.GetType(), Obj = (IConstructAggregates) o})
                .Where(o => o.Type.IsGenericType && o.Type.GetGenericTypeDefinition() == typeof(AggregateSnapshottingFactory<>))
                .Select(o => new {AggregateType = o.Type.GetGenericArguments().First(), Constructor = o.Obj})
                .ToArray();

            foreach (var factory in factories)
            {
                AggregateFromSnapshotsFactory.Register(factory.AggregateType,
                    m => factory.Constructor.Build(factory.GetType(), Guid.Empty, m));
            }
        }

        private async Task ConfigureContainer(IUnityContainer unityContainer, 
                                        IQuartzConfig quartzConfig, 
                                        ActorSystem actorSystem)
        {
            unityContainer.Register(new GridNodeContainerConfiguration(actorSystem,
                                                                       _transportMode,
                                                                       quartzConfig,
                                                                       DefaultTimeout));

            unityContainer.Register(_configuration);

            var persistentScheduler = System.ActorOf(System.DI().Props<SchedulingActor>(),nameof(SchedulingActor));
            unityContainer.RegisterInstance<IActorRef>(SchedulingActor.RegistrationName, persistentScheduler);

            var executor = await ConfigureCommandPipe();
            unityContainer.RegisterInstance<ICommandExecutor>(executor);
        }


        public async Task Stop()
        {
            if (_stopping) return;

            _log.Debug("GridDomain node {Id} is stopping", Id);
            _stopping = true;
            Container?.Dispose();

            try
            {
                if (_quartzScheduler != null && _quartzScheduler.IsShutdown == false)
                        _quartzScheduler.Shutdown();
            }
            catch (Exception ex)
            {
                _log.Warn($"Got error on quartz scheduler shutdown:{ex}");
            }

            if (System != null)
            {
                await System.Terminate();
                System.Dispose();
            }

            _log.Debug("GridDomain node {Id} stopped",Id);
        }

        public void Execute(params ICommand[] commands)
        {
            _commandExecutor.Execute(commands);
        }

        public void Execute<T>(T command, IMessageMetadata metadata) where T : ICommand
        {
            _commandExecutor.Execute(command, metadata);
        }

        public IMessageWaiter<Task<IWaitResults>> NewWaiter(TimeSpan? defaultTimeout = null)
        {
            return _waiterFactory.NewWaiter(defaultTimeout ?? DefaultTimeout);
        }

        public ICommandWaiter Prepare<T>(T cmd, IMessageMetadata metadata = null) where T : ICommand
        {
            return _commandExecutor.Prepare(cmd, metadata);
        }
    }
}