using System;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.DI.AutoFac;
using Akka.DI.Core;
using Akka.Util.Internal;
using Autofac;
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
using ActorTransportProxy = GridDomain.Node.Transports.Remote.ActorTransportProxy;

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

        private IContainer Container { get; set; }
        private ContainerBuilder _containerBuilder;

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
            _containerBuilder = new ContainerBuilder();

            System = Settings.ActorSystemFactory.Invoke();
            System.RegisterOnTermination(OnSystemTermination);
            Transport = new LocalAkkaEventBusTransport(System);
            _containerBuilder.Register(new GridNodeContainerConfiguration(Transport, Settings.Log));
            _waiterFactory = new MessageWaiterFactory(System, Transport, Settings.DefaultTimeout);

            Initializing.Invoke(this, this);

            System.InitDomainEventsSerialization(EventsAdaptersCatalog);
            _containerBuilder.Register(Settings.ContainerConfiguration);

            ActorTransportProxy = System.ActorOf(Props.Create(() => new ActorTransportProxy(Transport)), nameof(ActorTransportProxy));

            //var appInsightsConfig = AppInsightsConfigSection.Default ?? new DefaultAppInsightsConfiguration();
            //var perfCountersConfig = AppInsightsConfigSection.Default ?? new DefaultAppInsightsConfiguration();
            //
            //if(appInsightsConfig.IsEnabled)
            //{
            //    var monitor = new ActorAppInsightsMonitor(appInsightsConfig.Key);
            //    ActorMonitoringExtension.RegisterMonitor(System, monitor);
            //}
            //if(perfCountersConfig.IsEnabled)
            //    ActorMonitoringExtension.RegisterMonitor(System, new ActorPerformanceCountersMonitor());

            _commandExecutor = await CreateCommandExecutor();
            _containerBuilder.RegisterInstance(_commandExecutor);

            var domainBuilder = ConfigureDomain();
            Container = _containerBuilder.Build();
            System.AddDependencyResolver(new AutoFacDependencyResolver(Container, System));

            foreach(var m in domainBuilder.MessageRouteMaps)
                await m.Register(Pipe);

            var nodeController = System.ActorOf(Props.Create(() => new GridNodeController()), nameof(GridNodeController));

            await nodeController.Ask<GridNodeController.Started>(new GridNodeController.Start());

            Settings.Log.Debug("GridDomain node {Id} started at home {Home}", Id, System.Settings.Home);
        }

        private async Task<ICommandExecutor> CreateCommandExecutor()
        {
            Pipe = new CommandPipe(System);
            var commandExecutorActor = await Pipe.Init(_containerBuilder);
            return new AkkaCommandPipeExecutor(System, Transport, commandExecutorActor, Settings.DefaultTimeout);
        }

        private DomainBuilder ConfigureDomain()
        {
            var domainBuilder = new DomainBuilder();
            Settings.DomainConfigurations.ForEach(c => domainBuilder.Register(c));
            domainBuilder.ContainerConfigurations.ForEach(c => _containerBuilder.Register(c));
            return domainBuilder;
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