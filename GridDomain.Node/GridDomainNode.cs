using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
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
using GridDomain.Transport.Remote;
using GridDomain.Transport;
using GridDomain.Transport.Extension;
using Serilog;
using Serilog.Core;
using ICommand = GridDomain.CQRS.ICommand;

namespace GridDomain.Node
{
    public class GridDomainNode : IGridDomainNode
    {
        private ICommandExecutor _commandExecutor;

        private bool _stopping;
        private IMessageWaiterFactory _waiterFactory;
        internal CommandPipe Pipe;

        public GridDomainNode(IActorSystemFactory actorSystemFactory, params IDomainConfiguration[] domainConfigurations)
            :this(domainConfigurations,actorSystemFactory)
        { }
        public GridDomainNode(IActorSystemFactory actorSystemFactory, ILogger log, params IDomainConfiguration[] domainConfigurations)
            : this(domainConfigurations, actorSystemFactory, log)
        { }
        public GridDomainNode(IActorSystemFactory actorSystemFactory, ILogger log, TimeSpan timeout, params IDomainConfiguration[] domainConfigurations)
            : this(domainConfigurations, actorSystemFactory, log, timeout)
        { }

        public GridDomainNode(IEnumerable<IDomainConfiguration> domainConfigurations, IActorSystemFactory actorSystemFactory, ILogger log = null, TimeSpan? defaultTimeout = null)
        {
            DomainConfigurations = domainConfigurations.ToList();
            DefaultTimeout = defaultTimeout ?? TimeSpan.FromSeconds(10);
            _logger = log ?? new DefaultLoggerConfiguration().CreateLogger().ForContext<GridDomainNode>(); 
            _actorSystemFactory = actorSystemFactory;
        }

        public EventsAdaptersCatalog EventsAdaptersCatalog { get; private set; }
        public IActorTransport Transport { get; private set; }
        public ActorSystem System { get; private set; }
        private IActorRef ActorTransportProxy { get; set; }

        private IContainer Container { get; set; }
        private ContainerBuilder _containerBuilder;
        private readonly IActorSystemFactory _actorSystemFactory;
        private readonly ILogger _logger;
        internal readonly List<IDomainConfiguration> DomainConfigurations;
        public TimeSpan DefaultTimeout { get; }

        public Guid Id { get; } = Guid.NewGuid();
        public event EventHandler<GridDomainNode> Initializing = delegate { };

        public Task Execute(ICommand command, IMessageMetadata metadata = null)
        {
            return _commandExecutor.Execute(command, metadata);
        }

        public IMessageWaiter<Task<IWaitResult>> NewExplicitWaiter(TimeSpan? defaultTimeout = null)
        {
            return _waiterFactory.NewExplicitWaiter(defaultTimeout ?? DefaultTimeout);
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
            _logger.Debug("grid node Actor system terminated");
        }

        public async Task Start()
        {
            _logger.Debug("Starting GridDomain node {Id}", Id);

            _stopping = false;
            EventsAdaptersCatalog = new EventsAdaptersCatalog();
            _containerBuilder = new ContainerBuilder();

            System = _actorSystemFactory.Create();
            System.RegisterOnTermination(OnSystemTermination);

            System.InitLocalTransportExtension();
            Transport = System.GetTransport();

            _containerBuilder.Register(new GridNodeContainerConfiguration(Transport, _logger));
            _waiterFactory = new MessageWaiterFactory(System, Transport, DefaultTimeout);

            Initializing.Invoke(this, this);

            System.InitDomainEventsSerialization(EventsAdaptersCatalog);

            ActorTransportProxy = System.ActorOf(Props.Create(() => new LocalTransportProxyActor()), nameof(ActorTransportProxy));

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

            var domainBuilder = CreateDomainBuilder();
            domainBuilder.Configure(_containerBuilder);

            Container = _containerBuilder.Build();
            System.AddDependencyResolver(new AutoFacDependencyResolver(Container, System));
            domainBuilder.Configure(Pipe);
            var nodeController = System.ActorOf(Props.Create(() => new GridNodeController(Pipe.CommandExecutor,ActorTransportProxy)), nameof(GridNodeController));

            await nodeController.Ask<GridNodeController.Alive>(GridNodeController.HeartBeat.Instance);

            _logger.Debug("GridDomain node {Id} started at home {Home}", Id, System.Settings.Home);
        }

        private async Task<ICommandExecutor> CreateCommandExecutor()
        {
            Pipe = new CommandPipe(System);
            var commandExecutorActor = await Pipe.Init(_containerBuilder);
            return new AkkaCommandPipeExecutor(System, Transport, commandExecutorActor, DefaultTimeout);
        }

        private DomainBuilder CreateDomainBuilder()
        {
            var domainBuilder = new DomainBuilder();
            if (!DomainConfigurations.Any())
                throw new NoDomainConfigurationException();
            DomainConfigurations.ForEach(c => domainBuilder.Register(c));
            return domainBuilder;
        }

        public async Task Stop()
        {
            if(_stopping)
                return;

            _logger.Debug("GridDomain node {Id} is stopping", Id);
            _stopping = true;

            if(System != null)
            {
                await System.Terminate();
                System.Dispose();
            }
            System = null;
            Container?.Dispose();
            _logger.Debug("GridDomain node {Id} stopped", Id);
        }

        public IMessageWaiter<Task<IWaitResult>> NewWaiter(TimeSpan? defaultTimeout = null)
        {
            return _waiterFactory.NewWaiter(defaultTimeout);
        }
    }

    internal class NoDomainConfigurationException : Exception { }
}