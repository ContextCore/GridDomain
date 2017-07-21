using System;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.DI.Core;
using Akka.DI.Unity;
using Akka.Monitoring;
using Akka.Monitoring.ApplicationInsights;
using Akka.Monitoring.PerformanceCounters;
using Akka.Util.Internal;
using GridDomain.Common;
using GridDomain.Configuration;
using GridDomain.CQRS;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Adapters;
using GridDomain.EventSourcing.CommonDomain;
using GridDomain.Node.Actors;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Node.Serializers;
using GridDomain.Node.Transports;
using GridDomain.Scheduling;
using GridDomain.Scheduling.FutureEvents;
using Microsoft.Practices.Unity;
using ActorTransportProxy = GridDomain.Node.Transports.Remote.ActorTransportProxy;
using IScheduler = Quartz.IScheduler;

namespace GridDomain.Node
{
    public class GridDomainNode : IGridDomainNode
    {
        private ICommandExecutor _commandExecutor;

        private bool _stopping;
        private IMessageWaiterFactory _waiterFactory;
        internal CommandPipe Pipe;
        public GridDomainNode(NodeSettings settings)
        {
            Settings = settings;
        }

        public NodeSettings Settings { get; }
        public EventsAdaptersCatalog EventsAdaptersCatalog { get; private set; }
        public IActorTransport Transport { get; private set; }
        public ActorSystem System { get; private set; }
        private IActorRef ActorTransportProxy { get; set; }

        private IUnityContainer Container { get; set; }

        public Guid Id { get; } = Guid.NewGuid();
        public event EventHandler<GridDomainNode> Initializing = delegate { };

        public Task Execute(ICommand command, IMessageMetadata metadata = null)
        {
            return _commandExecutor.Execute(command, metadata);
        }

        public IMessageWaiter<Task<IWaitResult>> NewExplicitWaiter(TimeSpan? defaultTimeout = null)
        {
            return _waiterFactory.NewExplicitWaiter(defaultTimeout ?? Settings.DefaultTimeout);
        }

        public ICommandWaiter Prepare<T>(T cmd, IMessageMetadata metadata = null) where T : ICommand
        {
            return _commandExecutor.Prepare(cmd, metadata);
        }

        public void Dispose()
        {
            Stop().Wait();
        }

        private void OnSystemTermination()
        {
            Settings.Log.Debug("grid node Actor system terminated");
        }

        public async Task Start()
        {
            Settings.Log.Debug("Starting GridDomain node {Id}", Id);

            _stopping = false;
            EventsAdaptersCatalog = new EventsAdaptersCatalog();
            Container = new UnityContainer();

            Initializing.Invoke(this, this);

            System = Settings.ActorSystemFactory.Invoke();
            Transport = new LocalAkkaEventBusTransport(System);
            System.InitDomainEventsSerialization(EventsAdaptersCatalog);

            System.RegisterOnTermination(OnSystemTermination);

            NodeSettings settings = Settings;
            Container.Register(new GridNodeContainerConfiguration(Transport, settings.Log));
            Container.Register(Settings.ContainerConfiguration);
            System.AddDependencyResolver(new UnityDependencyResolver(Container, System));



            _waiterFactory = new MessageWaiterFactory(System, Transport, Settings.DefaultTimeout);


            ActorTransportProxy = System.ActorOf(Props.Create(() => new ActorTransportProxy(Transport)),
                                                 nameof(Transports.Remote.ActorTransportProxy));


            var appInsightsConfig = AppInsightsConfigSection.Default ?? new DefaultAppInsightsConfiguration();
            var perfCountersConfig = AppInsightsConfigSection.Default ?? new DefaultAppInsightsConfiguration();

            if(appInsightsConfig.IsEnabled)
            {
                var monitor = new ActorAppInsightsMonitor(appInsightsConfig.Key);
                ActorMonitoringExtension.RegisterMonitor(System, monitor);
            }
            if(perfCountersConfig.IsEnabled)
                ActorMonitoringExtension.RegisterMonitor(System, new ActorPerformanceCountersMonitor());

            _commandExecutor = await CreateCommandExecutor();

            var ext = System.InitSchedulingExtension(Settings.QuartzConfig, Settings.Log, Transport, _commandExecutor);
            Settings.Add(new FutureAggregateHandlersDomainConfiguration(ext.SchedulingActor));

            await ConfigureDomain();

            Container.RegisterInstance(_commandExecutor);


            var props = System.DI().Props<GridNodeController>();
            var nodeController = System.ActorOf(props, nameof(GridNodeController));

            await nodeController.Ask<GridNodeController.Started>(new GridNodeController.Start());

            Settings.Log.Debug("GridDomain node {Id} started at home {Home}", Id, System.Settings.Home);
        }

        private async Task<ICommandExecutor> CreateCommandExecutor()
        {
            Pipe = new CommandPipe(System, Container);
            var commandExecutorActor = await Pipe.Init();
            return new AkkaCommandPipeExecutor(System, Transport, commandExecutorActor, Settings.DefaultTimeout);
        }

        private async Task ConfigureDomain()
        {
            var domainBuilder = new DomainBuilder();
            Settings.DomainConfigurations.ForEach(c => domainBuilder.Register(c));
            domainBuilder.ContainerConfigurations.ForEach(c => Container.Register(c));
            foreach(var m in domainBuilder.MessageRouteMaps)
                await m.Register(Pipe);
        }

        public async Task Stop()
        {
            if(_stopping)
                return;

            Settings.Log.Debug("GridDomain node {Id} is stopping", Id);
            _stopping = true;

            if(System != null)
            {
                await System.Terminate();
                System.Dispose();
            }
            System = null;
            Container?.Dispose();
            Settings.Log.Debug("GridDomain node {Id} stopped", Id);
        }

        public IMessageWaiter<Task<IWaitResult>> NewWaiter(TimeSpan? defaultTimeout = null)
        {
            return _waiterFactory.NewWaiter(defaultTimeout);
        }
    }
}