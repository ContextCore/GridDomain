using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using Akka;
using Akka.Actor;
using Akka.DI.Core;
using Akka.DI.Unity;
using Akka.Event;
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
using GridDomain.Scheduling.Quartz.Retry;
using Microsoft.Practices.Unity;
using Serilog;

namespace GridDomain.Node
{
    
    public class NodeSettings
    {
        public IContainerConfiguration Configuration { get; } = new EmptyContainerConfiguration();
        public IMessageRouteMap MessageRouting { get; } = new EmptyRouteMap();
        public Func<ActorSystem[]> ActorSystemFactory { get; } = () => new[] {ActorSystem.Create("defaultSystem")};

        public ILogger Log { get; set; } = new DefaultLoggerConfiguration().CreateLogger()
                                                                           .ForContext<GridDomainNode>();
        public TimeSpan DefaultTimeout { get; set; } = TimeSpan.FromSeconds(10);
        public IQuartzConfig QuartzConfig { get; set; } = new InMemoryQuartzConfig();
        public IRetrySettings QuartzJobRetrySettings { get; set; } = new InMemoryRetrySettings();

        public NodeSettings(IContainerConfiguration configuration=null,
                            IMessageRouteMap messageRouting = null,
                            Func<ActorSystem[]> actorSystemFactory = null)
        {
            ActorSystemFactory = actorSystemFactory ?? ActorSystemFactory;
            MessageRouting = messageRouting ?? MessageRouting;
            Configuration = configuration ?? Configuration;
        }
    }


    public class GridDomainNode : IGridDomainNode
    {
        private TransportMode _transportMode;
        private ActorSystem[] Systems;

        private Quartz.IScheduler _quartzScheduler;
        private IMessageWaiterFactory _waiterFactory;
        private ICommandExecutor _commandExecutor;
        internal CommandPipeBuilder Pipe;
        bool _stopping = false;
        public NodeSettings Settings { get; }
        public EventsAdaptersCatalog EventsAdaptersCatalog { get; } = new EventsAdaptersCatalog();
        private AggregatesSnapshotsFactory AggregateFromSnapshotsFactory { get; } = new AggregatesSnapshotsFactory();
        public IActorTransport Transport { get; private set; }
        public ActorSystem System { get; private set; }
        private IActorRef EventBusForwarder { get; set; }

        public GridDomainNode(NodeSettings settings)
        {
            Settings = settings;
        }

        private void OnSystemTermination()
        {
            Settings.Log.Debug("grid node Actor system terminated");
        }

        public IUnityContainer Container { get; private set; }

        public Guid Id { get; } = Guid.NewGuid();

        public async Task Start()
        {
            _stopping = false;

            Container = new UnityContainer();
            Systems = Settings.ActorSystemFactory.Invoke();
            
            System = Systems.First();
            System.InitDomainEventsSerialization(EventsAdaptersCatalog);

            _transportMode = Systems.Length > 1 ? TransportMode.Cluster : TransportMode.Standalone;
            System.RegisterOnTermination(OnSystemTermination);

            IUnityContainer unityContainer = Container;
            unityContainer.Register(new GridNodeContainerConfiguration(System,
                                                                       _transportMode,
                                                                       Settings));

            unityContainer.Register(Settings.Configuration);

            Pipe = Container.Resolve<CommandPipeBuilder>();
            await Settings.MessageRouting.Register(Pipe);

            Transport = Container.Resolve<IActorTransport>();

            _quartzScheduler = Container.Resolve<Quartz.IScheduler>();
            _commandExecutor = Container.Resolve<ICommandExecutor>();
            _waiterFactory   = Container.Resolve<IMessageWaiterFactory>();

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

            Settings.Log.Debug("Launching GridDomain node {Id}",Id);

            var props = System.DI().Props<GridNodeController>();
            var nodeController = System.ActorOf(props,nameof(GridNodeController));

            await nodeController.Ask<GridNodeController.Started>(new GridNodeController.Start());

            Settings.Log.Debug("GridDomain node {Id} started at home {Home}", Id, System.Settings.Home);
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

        public async Task Stop()
        {
            if (_stopping) return;

            Settings.Log.Debug("GridDomain node {Id} is stopping", Id);
            _stopping = true;

            try
            {
                if (_quartzScheduler != null && _quartzScheduler.IsShutdown == false)
                    _quartzScheduler.Shutdown();
            }
            catch (Exception ex)
            {
                Settings.Log.Warning($"Got error on quartz scheduler shutdown:{ex}");
            }

            if (System != null)
            {
                await System.Terminate();
                System.Dispose();
            }

            Container?.Dispose();

            Settings.Log.Debug("GridDomain node {Id} stopped",Id);
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
            return _waiterFactory.NewWaiter(defaultTimeout ?? Settings.DefaultTimeout);
        }

        public ICommandWaiter Prepare<T>(T cmd, IMessageMetadata metadata = null) where T : ICommand
        {
            return _commandExecutor.Prepare(cmd, metadata);
        }
    }
}