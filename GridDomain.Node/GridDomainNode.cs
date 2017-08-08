using System;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.DI.Core;
using Akka.DI.Unity;
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
using Microsoft.Practices.Unity;
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

            System = Settings.ActorSystemFactory.Invoke();
            System.RegisterOnTermination(OnSystemTermination);
            Transport = new LocalAkkaEventBusTransport(System);
            Container.Register(new GridNodeContainerConfiguration(Transport, Settings.Log));
            System.AddDependencyResolver(new UnityDependencyResolver(Container, System));
            _waiterFactory = new MessageWaiterFactory(System, Transport, Settings.DefaultTimeout);
            ActorTransportProxy = System.ActorOf(Props.Create(() => new ActorTransportProxy(Transport)), nameof(ActorTransportProxy));

            Initializing.Invoke(this, this);

            System.InitDomainEventsSerialization(EventsAdaptersCatalog);
            Container.Register(Settings.ContainerConfiguration);

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